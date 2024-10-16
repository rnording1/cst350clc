namespace cst350groupapp.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Sex { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public byte[] Salt { get; set; }

        internal void SetPassword(string password)
        {
            // Generate salt
            byte[] salt = new byte[16];
            System.Security.Cryptography.RandomNumberGenerator.Fill(salt);
            Salt = salt;

            // Hash password
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
                byte[] saltedPasswordBytes = new byte[passwordBytes.Length + Salt.Length];
                System.Buffer.BlockCopy(passwordBytes, 0, saltedPasswordBytes, 0, passwordBytes.Length);
                System.Buffer.BlockCopy(Salt, 0, saltedPasswordBytes, passwordBytes.Length, Salt.Length);
                byte[] passwordHashBytes = sha256.ComputeHash(saltedPasswordBytes);
                PasswordHash = Convert.ToBase64String(passwordHashBytes);
            }
        }

        internal bool VerifyPassword(string password)
        {
            // Hash provided password with stored salt
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
                byte[] saltedPasswordBytes = new byte[passwordBytes.Length + Salt.Length];
                System.Buffer.BlockCopy(passwordBytes, 0, saltedPasswordBytes, 0, passwordBytes.Length);
                System.Buffer.BlockCopy(Salt, 0, saltedPasswordBytes, passwordBytes.Length, Salt.Length);
                byte[] passwordHashBytes = sha256.ComputeHash(saltedPasswordBytes);
                string hashedPassword = Convert.ToBase64String(passwordHashBytes);

                // Compare hashed passwords
                return PasswordHash == hashedPassword;
            }
        }
    }
}
