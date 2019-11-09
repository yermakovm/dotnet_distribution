using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
namespace DistributionAPI.Model
{ 
    public abstract class Person : IEntity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int z3kid { get; set; }
        public string name { get; set; }

       protected Person()
        {
            z3kid = 0;
            name = "noname";
        }

        public Person(int _id, string _name)
        {
            z3kid = _id;
            name = _name;
        }

    }
}