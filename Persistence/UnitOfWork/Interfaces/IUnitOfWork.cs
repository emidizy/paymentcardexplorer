using Persistence.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.UnitOfWork.Interfaces
{
    public interface IUnitOfWork
    {
        ICardInquiryRepository CardInquiries { get; }
        int SaveChanges();
        void Dispose();
    }
}
