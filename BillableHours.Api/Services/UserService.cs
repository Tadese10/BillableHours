using BillableHours.Common.Entities;
using BillableHours.Common.Helper;
using BillableHours.DAL.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BillableHours.Api.Services
{
    public interface IUserService
    {
        Response<User> Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetById(int id);
        Response<User> Create(User user, string password);
        Response<List<Invoice>> GetInvoiceList(string username);
        Response<Invoice> GetInvoiceById(int Id);
        Response<Invoice> AddInvoice(Invoice invoice);
        Response<List<Invoice>> AddInvoiceList(List<Invoice> invoice);

        void Update(User user, string password = null);
        void Delete(int id);
    }

    public class Response<T>
    {
        public string Message { get; set; }
        public bool OK { get; set; }
        public T Data { get; set; }
    }
    public class UserService : IUserService
    {
        private DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public Response<User> Authenticate(string username, string password)
        {
            Response<User> response = new Response<User>();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                response.Message = "Invalid Input(s)";
                response.OK = false;
                return null;
            }

            var user = _context.User.SingleOrDefault(x => x.Username == username);

            // check if username exists
            if (user == null)
            {
                response.Message = "Incorrect username/password";
                response.OK = false;
                return response;
            }
                //return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                response.Message = "Incorrect username/password";
                response.OK = false;
                return response;
            }

            // authentication successful
            response.Data = new User { CompanyEmail = user.CompanyEmail, CompanyName = user.CompanyName, Id = user.Id, Username = user.Username};
            response.OK = true;
            return response;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.User;
        }

        public User GetById(int id)
        {
            return _context.User.Find(id);
        }

        public Response<User> Create(User user, string password)
        {
            Response<User> response = new Response<User>();
            // validation
            if (string.IsNullOrWhiteSpace(password))
            {
                response.Message = "Password is required";
                response.OK = false;
                return response;
            }

            if (_context.User.Any(x => x.Username == user.Username))
            {
                response.Message = "Username \"" + user.Username + "\" is already taken";
                response.OK = false;
                return response;
            }

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.User.Add(user);
            _context.SaveChanges();
            response.OK = true;
            response.Data = new User { CompanyEmail = user.CompanyEmail, CompanyName = user.CompanyName, Id = user.Id };

            return response;
        }

        public Response<Invoice> AddInvoice(Invoice invoice)
        {
            Response<Invoice> response = new Response<Invoice>();
            _context.Invoice.Add(invoice);
            _context.SaveChanges();
            response.OK = true;
            response.Data = invoice;
            return response;
        }

        public Response<List<Invoice>> AddInvoiceList(List<Invoice> list)
        {
            Response<List<Invoice>> response = new Response<List<Invoice>>();
            _context.Invoice.AddRange(list);
            _context.SaveChanges();
            response.OK = true;
            response.Data = _context.Invoice.Where(s=>s.username == list[0].username).ToList();
            return response;
        }

        public void Update(User userParam, string password = null)
        {
            var user = _context.User.Find(userParam.Id);

            if (user == null)
                throw new AppException("User not found");

            if (userParam.Username != user.Username)
            {
                // username has changed so check if the new username is already taken
                if (_context.User.Any(x => x.Username == userParam.Username))
                    throw new AppException("Username " + userParam.Username + " is already taken");
            }

            // update user properties
            user.CompanyName = userParam.CompanyName;
            user.CompanyEmail = userParam.CompanyEmail;
            user.Username = userParam.Username;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.User.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.User.Find(id);
            if (user != null)
            {
                _context.User.Remove(user);
                _context.SaveChanges();
            }
        }

        // private helper methods

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new System.ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

        public Response<List<Invoice>> GetInvoiceList(string username)
        {
            Response<List<Invoice>> response = new Response<List<Invoice>>();
            response.OK = true;
            response.Data = _context.Invoice.Where(so => so.username.Equals(username)).ToList();
            return response;
        }

        public Response<Invoice> GetInvoiceById(int Id)
        {
            Response<Invoice> response = new Response<Invoice>();
            response.OK = true;
            response.Data = _context.Invoice.Where(so => so.Id.Equals(Id)).SingleOrDefault();
            return response;
        }
    }
}
