using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Menu.Contexts;

public class MenuContextFactory : IDesignTimeDbContextFactory<MenuContext>
{
    
    public MenuContext CreateDbContext(string[] args)
    {
    try{
        var optionsBuilder = new DbContextOptionsBuilder<MenuContext>();

        var connectionString =
            "Server=127.0.0.1;Port=3306;Database=MenuDb;User=rider;Password=password123;";

        optionsBuilder.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString)
            
        );
        
            Console.WriteLine($"Connection String: {connectionString}");
            
        
        return new MenuContext(optionsBuilder.Options);
     
    }   catch (Exception e){
        Console.WriteLine($"Error: {e.Message}");
        throw;
    } 
    }

}