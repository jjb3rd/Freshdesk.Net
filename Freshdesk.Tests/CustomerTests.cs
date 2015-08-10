/*
 * Copyright 2015 Beckersoft, Inc.
 *
 * Author(s):
 *  John Becker (john@beckersoft.com)
 *  
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using NUnit.Framework;

namespace Freshdesk.Tests
{
    [TestFixture]
    public class CustomerTests
    {
        Freshdesk.FreshdeskService freshdeskService = new Freshdesk.FreshdeskService(Settings.FreshdeskApiKey, Settings.FreshdeskUri);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Freshdesk")]
        [Test]
        public void FreshdeskCreateCustomer()
        {
            // create Organization with Support software integration
            Freshdesk.GetCustomerResponse customerResponse = freshdeskService.CreateCustomer(new Freshdesk.CreateCustomerRequest()
            {
                Customer = new Freshdesk.Customer()
                {
                    Name = "ACME Corp.",
                    Description = "The ACME Corporation"
                }
            });
            Assert.IsNotNull(customerResponse);
            Assert.IsNotNull(customerResponse.Customer);
            if(customerResponse.Customer.Id < 1)
            {
                Assert.Fail("Invalid customer id.");
            }
        }
    }
}
