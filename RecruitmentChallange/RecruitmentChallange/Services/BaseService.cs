using RecruitmentChallange.Models;
using System;

namespace RecruitmentChallange.Services
{
    public abstract class BaseService : IDisposable
    {
        protected readonly WebAppDBContext dbContext;
        private bool disposed;

        public BaseService(WebAppDBContext dbContext)
        {
            this.dbContext = dbContext;
            this.disposed = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
                if (disposing)
                    dbContext.Dispose();
            disposed = true;
        }
    }
}
