using Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repository.Interfaces
{
    public interface ICardInquiryRepository
    {
        List<CardInquiryLog> GetAllInquiries();
        Task AddCardInquiry(CardInquiryLog newInquiry);
        List<CardInquiryLog> GetNoOfHits();
        IEnumerable<CardInquiryLog> Filter(Expression<Func<CardInquiryLog, bool>> predicate);

    }
}
