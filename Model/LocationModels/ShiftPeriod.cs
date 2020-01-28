using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DistributionAPI.Model
{
    public class ShiftPeriod
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string XPath { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public ShiftPeriod()
        {

        }
        public ShiftPeriod(string path, string start, string end)
        {
            XPath = path;
            Start = start;
            End = end;
        }
    }
    
}
