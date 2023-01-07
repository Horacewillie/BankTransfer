﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Domain.Models
{
    public abstract class QueueMessage
    {
        public abstract string QueueName { get; }
        public virtual string Label { get; }
    }
}
