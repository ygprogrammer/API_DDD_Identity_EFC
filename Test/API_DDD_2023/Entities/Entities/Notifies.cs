using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Entities
{
    public class Notifies
    {
        public Notifies()
        {
            Notitycoes = new List<Notifies>();
        }

        [NotMapped]
        public string NameProperty { get; set; }

        [NotMapped]
        public string Message { get; set; }

        [NotMapped]
        public List<Notifies> Notitycoes { get; set; }

        public bool ValidatePropertyString(string value, string nameProperty)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(nameProperty)) 
            {
                Notitycoes.Add(new Notifies
                {
                    Message = "Campo Obrigatório",
                    NameProperty = nameProperty
                });

                return false;
            }
            return true;
        }

        public bool ValidatePropertyInt(int value, string nameProperty)
        {
            if (value < 1 || string.IsNullOrWhiteSpace(nameProperty))
            {
                Notitycoes.Add(new Notifies
                {
                    Message = "Campo Obrigatório",
                    NameProperty = nameProperty
                });

                return false;
            }
            return true;
        }

    }
}
