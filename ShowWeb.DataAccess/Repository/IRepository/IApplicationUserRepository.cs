﻿using ShowWeb.Models;

namespace ShowWeb.DataAccess.Repository.IRepository;

public interface IApplicationUserRepository : IRepository<ApplicationUser>
{
    void Update(ApplicationUser applicationUser);
}