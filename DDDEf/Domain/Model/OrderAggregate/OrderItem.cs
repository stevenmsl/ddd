﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DDDEf.Domain.SeedWork;

namespace DDDEf.Domain.Model.OrderAggregate
{
    public class OrderItem : Entity
    {
        // DDD Patterns comment
        // Using private fields is a much better encapsulation
        // aligned with DDD Aggregates and Domain Entities (Instead of properties and property collections)
        private string _productName;
        private string _pictureUrl;
        private decimal _unitPrice;
        private decimal _discount;
        private int _units;

        public int ProductId { get; private set; }

        protected OrderItem() { }

        public OrderItem(int productId, string productName, decimal unitPrice, decimal discount, string PictureUrl, int units = 1)
        {
            if (units <= 0)
            {
                throw new Exception("Invalid number of units");
            }

            if ((unitPrice * units) < discount)
            {
                throw new Exception("The total of order item is lower than applied discount");
            }

            ProductId = productId;

            _productName = productName;
            _unitPrice = unitPrice;
            _discount = discount;
            _units = units;
            _pictureUrl = PictureUrl;
        }

        public string GetPictureUri() => _pictureUrl;

        public decimal GetCurrentDiscount()
        {
            return _discount;
        }

        public int GetUnits()
        {
            return _units;
        }

        public decimal GetUnitPrice()
        {
            return _unitPrice;
        }

        public string GetOrderItemProductName() => _productName;

        public void SetNewDiscount(decimal discount)
        {
            if (discount < 0)
            {
                throw new Exception("Discount is not valid");
            }

            _discount = discount;
        }

        public void AddUnits(int units)
        {
            if (units < 0)
            {
                throw new Exception("Invalid units");
            }

            _units += units;
        }
    }
}