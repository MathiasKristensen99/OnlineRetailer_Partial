using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Data;
using OrderApi.Infrastructure;
using SharedModels;
using RestSharp;
using static SharedModels.Order;

namespace OrderApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IRepository<Order> _repository;
        private readonly IMessagePublisher _messagePublisher;

        public OrdersController(IRepository<Order> repos, IMessagePublisher messagePublisher)
        {
            _repository = repos;
            _messagePublisher = messagePublisher;
        }

        // GET: orders
        [HttpGet]
        public IEnumerable<Order> Get()
        {
            return _repository.GetAll();
        }

        // GET orders/5
        [HttpGet("{id}", Name = "GetOrder")]
        public IActionResult Get(int id)
        {
            var item = _repository.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST orders
        [HttpPost]
        public IActionResult Post([FromBody]Order order)
        {
            if (order == null)
            {
                return BadRequest();
            }

            try
            {
                order.Status = Order.OrderStatus.tentative;
                var newOrder = _repository.Add(order);

                _messagePublisher.PublishOrderCreatedMessage(newOrder.CustomerId, newOrder.Id, newOrder.OrderLines);

                var completed = false;
                while (!completed)
                {
                    var tentativeOrder = _repository.Get(newOrder.Id);
                    if (tentativeOrder.Status == Order.OrderStatus.completed)
                    {
                        completed = true;
                    }
                    Thread.Sleep(100);
                }

                return CreatedAtRoute("GetOrder", new { id = newOrder.Id }, newOrder);
            }
            catch (Exception e)
            {
                return StatusCode(500, "An error happened. Try again.");
            }
        }

        // PUT orders/5/cancel
        // This action method cancels an order and publishes an OrderStatusChangedMessage
        // with topic set to "cancelled".
        [HttpPut("{id}/cancel")]
        public IActionResult Cancel(int id)
        {
            try
            {
                _repository.Edit(new Order
                {
                    Id = id,
                    Status = Order.OrderStatus.cancelled
                });
                _messagePublisher.PublishOrderCancelledMessage(id);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, "An error happened. Try again.");
            }
        }

        // PUT orders/5/ship
        // This action method ships an order and publishes an OrderStatusChangedMessage.
        // with topic set to "shipped".
        [HttpPut("{id}/ship")]
        public IActionResult Ship(int id)
        {
            try
            {
                _repository.Edit(new Order
                {
                    Id = id,
                    Status = Order.OrderStatus.shipped
                });
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, "An error happened. Try again.");
            }
        }

        // PUT orders/5/pay
        // This action method marks an order as paid and publishes a CreditStandingChangedMessage
        // (which have not yet been implemented), if the credit standing changes.
        [HttpPut("{id}/pay")]
        public IActionResult Pay(int id)
        {
            try
            {
                var order = _repository.Get(id);

                _messagePublisher.PublishOrderPaidMessage(order.Id, order.CustomerId);

                bool completed = false;
                while (!completed)
                {
                    var tentativeOrder = _repository.Get(order.Id);
                    if (tentativeOrder.Status == OrderStatus.paid)
                    {
                        completed = true;
                    }
                    Thread.Sleep(200);
                }

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, "An error happened. Try again.");
            }
        }
    }
}
