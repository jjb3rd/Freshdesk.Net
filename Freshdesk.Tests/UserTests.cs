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
    public class UserTests
    {
        Freshdesk.FreshdeskService freshdeskService = new Freshdesk.FreshdeskService(Settings.FreshdeskApiKey, Settings.FreshdeskUri);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Freshdesk"), Test]
        public void FreshdeskCreateContact()
        {
            Freshdesk.GetUserResponse userResponse = freshdeskService.CreateUser(new Freshdesk.CreateUserRequest()
            {
                User = new Freshdesk.User()
                {
                    Name = "Wil E. Coyote",
                    Email = "wilecoyote@acme.com"
                }
            });
            Assert.IsNotNull(userResponse);
            Assert.IsNotNull(userResponse.User);

        }

    }
}
