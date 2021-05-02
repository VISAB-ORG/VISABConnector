# VISABConnector
VISABConnector component that can be used for Http based communication with the VISAB WebApi

### Sample usage in Unity Project

```csharp
using VISABConnector;

public class MyGameStatistics : IVISABStatistics 
{
    public string Game => "MyGame";
    
    public int Turn { get; set; }
    
    public int Health { get; set; }
}

public class SendDataToVISAB 
{
    private IVISABSession session;
    
    // Get a session instance
    public async Task SetSession() 
    {
        // Get a VISAB session object
        session = await VISABApi.InitiateSession("MyGame");
        
        // If the session wasn't created, check if VISAB is running and if not, start it.
        if (session == default) 
        {
            var pingResponse = await VISABApi.IsApiReachable();
            if (!pingResponse.IsSuccess)
            {
                // Start VISAB from the PATH system variable
                VISABApi.StartVISAB("visab");
                
                // Wait a second for the process to start
                await Task.Delay(1000);
           }
            
            // Try getting a session again
            session = await VISABApi.InitiateSession("MyGame");
        }
    }
    
    // Send Statistics
    public async Task<bool> SendStatistics(MyGameStatistics statistics) 
    {
        var response = await session.SendStatistics(statistics);
        
        return response.IsSuccess;
    }
    
    // Close the session when you're done
    public async Task<bool> CloseSession()
    {
        var response = await session.CloseSession();
        
        return response.IsSuccess;
    }
}
```
