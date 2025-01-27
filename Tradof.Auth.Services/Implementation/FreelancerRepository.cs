﻿using Tradof.Auth.Services.Interfaces;
using Tradof.Data.Entities;
using Tradof.EntityFramework.DataBase_Context;

namespace Tradof.Auth.Services.Implementation
{
    public class FreelancerRepository : IFreelancerRepository
    {
        private readonly TradofDbContext _context;

        public FreelancerRepository(TradofDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Freelancer freelancer)
        {
            await _context.Freelancers.AddAsync(freelancer);
            await _context.SaveChangesAsync();
        }
    }
}
