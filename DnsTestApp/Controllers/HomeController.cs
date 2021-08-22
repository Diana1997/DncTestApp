using System;
using System.Collections.Generic;
using System.Linq;
using DnsTestApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace DnsTestApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {

        [HttpPost]
        public IActionResult Search([FromBody] IList<Guid> idClients)
        {
            if (!idClients.Any())
            {
                return BadRequest();
            }

            idClients = idClients.Distinct().ToList();

            IDictionary<Guid, IList<Guid>> dictionary =
                ListToDictionary(new RGDialogsClients().Init());


            if (!dictionary.Any())
            {
                return Ok(Guid.Empty);
            }

            Guid result = Guid.Empty;

            bool status = true;
            foreach (var item in dictionary)
            {
                if (item.Value.Count == idClients.Count)
                {
                    foreach (var idClient in idClients)
                    {
                        if (!item.Value.Contains(idClient))
                        {
                            status = false;
                            break;
                        }
                    }

                    if (status)
                    {
                        result = item.Key;
                        break;
                    }
                    status = true;
                }
            }
            return Ok(result);
        }


        private IDictionary<Guid, IList<Guid>> ListToDictionary(IList<RGDialogsClients> list)
        {
            IDictionary<Guid, IList<Guid>> dictionary = new Dictionary<Guid, IList<Guid>>();
            foreach (var item in list)
            {
                if (dictionary.ContainsKey(item.IDRGDialog))
                {
                    IList<Guid> temp = dictionary[item.IDRGDialog];
                    if (!temp.Contains(item.IDClient))
                    {
                        temp.Add(item.IDClient);
                        dictionary[item.IDRGDialog] = temp;
                    }
                }
                else
                {
                    dictionary.Add(
                        new KeyValuePair<Guid, IList<Guid>>(
                                item.IDRGDialog, new List<Guid>() { item.IDClient }
                        ));
                }
            }
            return dictionary;
        }

    }

}