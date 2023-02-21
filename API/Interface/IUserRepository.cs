﻿using API.Entities;

namespace API.Interface
{
    public interface IUserRepository
    {

        void Update(AppUser user);

        Task<bool> SaveAllAsync();

        Task<IEnumerable<AppUser>> GetUsersAsync();

        Task<AppUser> GetByIdAsync(int id);

        Task<AppUser> GetByNameAsync(string username);



    }
}
