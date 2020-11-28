using Persistence.Entities;
using Persistence.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
