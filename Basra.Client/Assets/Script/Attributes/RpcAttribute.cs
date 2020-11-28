namespace Basra.Client
{
    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class RpcAttribute : System.Attribute
    {
        // // See the attribute guidelines at
        // //  http://go.microsoft.com/fwlink/?LinkId=85236
        // readonly string positionalString;

        // // This is a positional argument
        // public RpcAttribute(string positionalString)
        // {
        //     this.positionalString = positionalString;

        //     // TODO: Implement code here
        //     throw new System.NotImplementedException();
        // }

        // public string PositionalString
        // {
        //     get { return positionalString; }
        // }

        // // This is a named argument
        // public int NamedInt { get; set; }
    }
}