// (c) American Software, Inc. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace Amsoftware.E2E.Supply.WebApiHelloWorldService.Models
{
    public class EoqRequest
    {
        /// <summary>
        /// Annual demand quantity
        ///
        /// Valid input is zero or positive whole number (integer)
        /// </summary>
        [Range(0, long.MaxValue)]
        [Required]
        public long D { get; set; }

        /// <summary>
        /// Fixed cost per order (not per unit)
        ///
        /// Valid input is non-zero, positive number
        /// </summary>
        [Range(double.Epsilon, double.MaxValue)]
        [Required]
        public double K { get; set; }

        /// <summary>
        /// Annual holding cost per unit
        ///
        /// Valid input is non-zero, positive number
        /// Validation for 0 is done in controller method for demo purposes
        /// </summary>
        [Range(0, double.MaxValue)]
        [Required]
        public double h { get; set; }
    }
}
