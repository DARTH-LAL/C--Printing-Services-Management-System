using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OopFinalProject
{
    public class PrintingService
    {
        private int serviceId;
        private string serviceType;
        private string size;
        private decimal feesPerUnit;
        private decimal discountThreshold;
        private decimal discountRate;

        // Public properties to access the private fields
        public int ServiceId { get => serviceId; }
        public string ServiceType { get => serviceType; }
        public string Size { get => size; }
        public decimal FeesPerUnit { get => feesPerUnit; }
        public decimal DiscountThreshold { get => discountThreshold; }
        public decimal DiscountRate { get => discountRate; }

        // Constructor to initialize the properties
        public PrintingService(int id, string type, string Size, decimal fees, decimal DiscountThreshold, decimal DiscountRate)
        {
            serviceId = id;
            serviceType = type;
            size = Size;
            feesPerUnit = fees;
            discountThreshold = DiscountThreshold;
            discountRate = DiscountRate;
        }

        // Public method to calculate total cost
        public decimal CalculateTotalCost(int quantity, bool isUrgent)
        {
            decimal baseCost = feesPerUnit * quantity;
            decimal discount = 0;
            if (quantity > discountThreshold)
            {
                discount = baseCost * discountRate;
            }
            decimal totalCost = baseCost - discount;
            if (isUrgent)
            {
                totalCost += totalCost * 0.30M; 
            }
            return totalCost;
        }
    }

}
