﻿using API.DTO;
using API.Entities;
using API.Helper;
using API.Interface;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Identity.Client;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
     

        public async Task<AppUser> GetByIdAsync(int id)
        {
           var user = await _context.Users.FindAsync(id);

           return user == null ? null : user;

        }

        public async Task<AppUser> GetByNameAsync(string username)
        {
            //eagerly load the photos as well.
            var user = await _context.Users.Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username.ToLower());

            return user == null ? null : user;

        }

        public async Task<MemberDTO?> GetMemberAsync(string username)
        {
            return await _context.Users
                .Where(x => x.UserName == username)
                .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();

            //exclude the user making the request from query.
            query = query.Where(x => x.UserName != userParams.CurrentUsername);
            
            //exclude same gender
            //query = query.Where(x => x.Gender == userParams.Gender);

            return await PagedList<MemberDTO>.CreateAsync(
                query.ProjectTo<MemberDTO>(_mapper.ConfigurationProvider), 
                userParams.PageNumber, 
                userParams.PageSize);
                
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            var users = await _context.Users.Include(x => x.Photos).ToListAsync();

            return users == null ? null : users;
        }

        public async Task<bool> SaveAllAsync()
        {
            //turn into boolean
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        public void AddUser(AppUser user)
        {
            _context.Users.Add(user);
        }

        public void DeleteUser(AppUser user)
        {
            _context.Entry(user).State = EntityState.Deleted;
        }

        public async Task<bool> UserExist(string userName)
        {
            if (userName!= null) return await _context.Users.AnyAsync(x => x.UserName == userName.ToLower());

            return false;
        }
    }
}
