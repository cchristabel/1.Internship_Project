using CsvHelper;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SecureMVCApp.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;

namespace SecureMVCApp.Services
{
    public class CsvUserImportService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager; // add UserManager

        public CsvUserImportService(IConfiguration configuration, UserManager<IdentityUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager; // initialize UserManager
        }

        public void ImportUsersFromCsv(string csvFilePath)
        {
            using (var reader = new StreamReader(csvFilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<NewUserCsvModel>().ToList();

                // Validate CSV format
                if (!ValidateCsv(records))
                {
                    throw new Exception("Invalid CSV file format");
                }

                // load existing users from the JSON database
                var jsonFilePath = "users.json";
                var usersJson = File.ReadAllText(jsonFilePath);
                var usersData = JsonConvert.DeserializeObject<UsersConfig>(usersJson);

                foreach (var newUser in records)
                {
                    // check for duplicates based on email
                    if (usersData.Users.Any(u => u.Email == newUser.Email))
                    {
                        Console.WriteLine($"Duplicate user found: {newUser.Email}. Skipping...");
                        continue; // Skip duplicates
                    }

                    // create new user object and add to the list
                    var newUserConfig = new UserConfig
                    {
                        FirstName = newUser.FirstName,
                        LastName = newUser.LastName,
                        Email = newUser.Email,
                        Password = newUser.Password,
                        Claims = new List<UserClaim>
                        {
                            new UserClaim { Type = "role", Value = newUser.role },
                            new UserClaim { Type = "Department", Value = newUser.Department },
                            new UserClaim { Type = "Board Title", Value = newUser.BoardTitle }
                        }
                    };

                    // create IdentityUser and save to database
                    var identityUser = new IdentityUser(newUserConfig.Email)
                    {
                        Email = newUserConfig.Email,
                        UserName = newUserConfig.Email
                    };

                    // allows new users to log in as soon as their credentials are added during the CSV import
                    var result = _userManager.CreateAsync(identityUser, newUserConfig.Password).Result;

                    if (result.Succeeded)
                    {
                        // add claims for the new user
                        foreach (var claim in newUserConfig.Claims)
                        {
                            _userManager.AddClaimAsync(identityUser, new Claim(claim.Type, claim.Value)).Wait();
                        }

                        // add userConfig to usersData for JSON persistence
                        usersData.Users.Add(newUserConfig);
                    }
                    else
                    {
                        Console.WriteLine($"Failed to create user: {newUserConfig.Email}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }

                // write the updated users back to the JSON file
                var updatedUsersJson = JsonConvert.SerializeObject(usersData, Formatting.Indented);
                File.WriteAllText(jsonFilePath, updatedUsersJson);
            }
        }

        private bool ValidateCsv(List<NewUserCsvModel> records)
        {
            // basic validation to check if required fields are not missing
            return records.All(r => !string.IsNullOrEmpty(r.FirstName) &&
                                    !string.IsNullOrEmpty(r.LastName) &&
                                    !string.IsNullOrEmpty(r.Email) &&
                                    !string.IsNullOrEmpty(r.Password));
        }
    }

    public class NewUserCsvModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string role { get; set; }
        public string Department { get; set; }
        public string BoardTitle { get; set; }
    }
}
