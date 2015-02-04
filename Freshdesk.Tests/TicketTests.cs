/*
 * Copyright 2014 Beckersoft, Inc.
 *
 * Author(s):
 *  John Becker (john@beckersoft.com)
 *  Oleg Shevchenko (shevchenko.oleg@outlook.com)
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
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Freshdesk.Tests
{
    [TestClass]
    public class TicketTests
    {
        Freshdesk.FreshdeskService freshdeskService = new Freshdesk.FreshdeskService(Settings.FreshdeskApiKey, Settings.FreshdeskUri);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Freshdesk"), TestMethod]
        public void TestFreshdeskCreateTicket()
        {
            // create Support Ticket for a followup demonstration with Support software integration
            Freshdesk.GetTicketResponse ticketResponse = freshdeskService.CreateTicket(new Freshdesk.CreateTicketRequest()
            {
                TicketInfo = new Freshdesk.CreateTicketInfo()
                {
                    Email = "wilecoyote@acme.com",
                    Subject = "ACME Corporation Initial Signup Demo",
                    Description = "Initial Product Demo",
                    Priority = 1,
                    Status = 2
                }
            });
            Assert.IsNotNull(ticketResponse);
            Assert.IsNotNull(ticketResponse.TicketInfo);

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Freshdesk"), TestMethod]
        public void TestFreshdeskCreateTicketWithAttachment()
        {
            // create Support Ticket for a followup demonstration with Support software integration
            var ticketResponse = freshdeskService.CreateTicketWithAttachment(new CreateTicketRequest
            {
                TicketInfo = new CreateTicketInfo
                {
                    Email = "demo@acme.com",
                    Subject = "Attachment test",
                    Description = "Description text",
                    Priority = 1,
                    Status = 2
                }
            }, new Collection<Attachment>
            {
                new Attachment
                {
                    Content = File.OpenRead("Freshdesk.dll"),
                    FileName = "Freshdesk.dll"
                },
                new Attachment
                {
                    Content = File.OpenRead("Freshdesk.pdb"),
                    FileName = "Freshdesk.pdb"
                }
            });            
            Assert.IsNotNull(ticketResponse);
            Assert.IsNotNull(ticketResponse.TicketInfo);
        }
    }
}
