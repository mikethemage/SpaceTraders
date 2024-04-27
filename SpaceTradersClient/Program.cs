namespace SpaceTradersClient;

internal class Program
{
    static async Task Main(string[] args)
    {
        //Initialisation:
        //Get API key from storage
        //If we have an API key, attempt login
        //If login failed - delete API key
        //If no API key, or login failed - register
        //If registration failed - exit

        //We now should be logged on

        //If we just registered:
        //We will have agent details, contract details, and ship details for one ship

        //Otherwise we have nothing:
        //need to get agent details
        //need to get contract details

        //Now we need to get:
        //full list of ships
        //List of Nav points for all systems containing ships

        //while(true)
        //Get all Idle ships
        //Foreach ship:
        //  Determine next order for ship
        //  Issue order
        //  Update next available time for ship
        //  Move on to next ship
        //Wait until next available time
    }
}
