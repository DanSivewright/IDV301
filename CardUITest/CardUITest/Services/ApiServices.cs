﻿using CardUITest.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CardUITest.Models;

namespace CardUITest.Services
{
    public class ApiServices
    {
        internal async Task<bool> CreateAsync(string firstName, string lastName, string emailAddress)
        {
            var client = new HttpClient();

            var model = new Models.Student
            {
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = emailAddress
            };
            var json = JsonConvert.SerializeObject(model);

            HttpContent content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var res = await client.PostAsync("https://10.0.0.2:44377/Student/Create", content);

            return res.IsSuccessStatusCode;
        }
    }
}