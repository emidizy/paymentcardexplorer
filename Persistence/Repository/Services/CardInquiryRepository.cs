using Persistence.Entities;
using Persistence.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repository.Services
{
    public class CardInquiryRepository : Repository<CardInquiryLog>, ICardInquiryRepository
    {
        public CardInquiryRepository(DatabaseContext dbContext) : base(dbContext)
        {

        }
        

        public List<CardInquiryLog> GetAllInquiries()
        {
            var transactions = GetAllRecords().ToList();
            return transactions;
        }

        public async Task AddCardInquiry(CardInquiryLog newInquiry)
        {
            await AddRecord(newInquiry);
        }

        public IEnumerable<CardInquiryLog> Filter(Expression<Func<CardInquiryLog, bool>> predicate)
        {
            var inquiries = Find(predicate)
                .ToList();
            return inquiries;
        }

        public List<CardInquiryLog> GetNoOfHits()
        {
            var records = GetAllInquiries().ToList();
            var groupedList = records.GroupBy(col => col.IIN)

                .Select(y => new CardInquiryLog() {
                    IIN = y.Key,
                    NoOfHit = y.Sum(s => s.NoOfHit),
                    InquiryDate = DateTime.Now,
                    Status = null
                })
                .ToList();

            return groupedList;
        }
    }
}
