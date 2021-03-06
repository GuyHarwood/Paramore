﻿#region Licence
/* The MIT License (MIT)
Copyright © 2014 Ian Cooper <ian_hammond_cooper@yahoo.co.uk>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the “Software”), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE. */
#endregion

using System;
using Common.Logging;
using Polly;

namespace paramore.brighter.commandprocessor.exceptionpolicy.Handlers
{
    public class ExceptionPolicyHandler<TRequest> : RequestHandler<TRequest> where TRequest : class, IRequest
    {
        private Policy policy;

        public ExceptionPolicyHandler(ILog logger) : base(logger)
        {}

        public override void InitializeFromAttributeParams(params object[] initializerList)
        {
            //we expect the first and only parameter to be a string
            var policyName = (string) initializerList[0];
            policy = Context.Policies.Get(policyName);
            if (policy == null)
                throw new ArgumentException("Could not find the policy for this attribute, did you register it with the command processor's container", "initializerList");
        }

        public override TRequest Handle(TRequest command)
        {
            return policy.Execute(() => base.Handle(command));
        }
    }
}
