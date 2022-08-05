// See https://aka.ms/new-console-template for more information
using System.Text;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.CommandLine;
using RestSharp;
var rcmd=new RootCommand();
var gcmd=new Command("Get","GET method");
gcmd.AddAlias("GET");
gcmd.AddAlias("get");
var pcmd=new Command("Post","POST method");
pcmd.AddAlias("POST");
pcmd.AddAlias("post");
var urla=new Argument<string>(name:"url","url to connect");
var pp=new Argument<string[]?>(name:"parameters","pp") {Arity=ArgumentArity.ZeroOrMore};
gcmd.AddArgument(urla);
gcmd.AddArgument(pp);
pcmd.AddArgument(urla);
pcmd.AddArgument(pp);
rcmd.AddCommand(pcmd);
rcmd.AddCommand(gcmd);
gcmd.SetHandler((url,pp) =>{
	GetCmd(url,pp,"get");
},urla,pp);
pcmd.SetHandler((url,pp) => {
	GetCmd(url,pp,"post");
},urla,pp);
rcmd.Invoke(args);
static void GetCmd(string url,string[]? args,string method) {
var client=new RestClient(url);
var req=new RestRequest("",Method.Get);
if(method=="post")
	req=new RestRequest("",Method.Post);
foreach(var arg in args!) 
{
	if(isQuery(arg)) 
	{
		var ss=arg.Split("==",2,StringSplitOptions.RemoveEmptyEntries);
		if(ss.Length==2) 
		{
			req.AddParameter(ss[0],ss[1]);
		}
	}else if (isHeader(arg))
	{
		var ss=arg.Split(":",2,StringSplitOptions.RemoveEmptyEntries);
		if(ss.Length==2) 
		{
			req.AddHeader(ss[0],ss[1]);
		}
		else 
		{
			//req.Parameters.Remove(ss[0],null);
		}
	}
	else if (isForm(arg) && method=="post")
	{
		var ss=arg.Split("=",2,StringSplitOptions.RemoveEmptyEntries);
		if(ss.Length==2) 
		{
			req.AddParameter(ss[0],ss[1]);
		}
	}
	else if (isCookie(arg)) 
	{
		var ss=arg.Split("#",2,StringSplitOptions.RemoveEmptyEntries);
		if(ss.Length==2) 
		{
			var uri=new Uri(url);
			Console.WriteLine("cookie");
			client.AddCookie(ss[0],ss[1],"",uri.DnsSafeHost);
		}
	}
		
}
var resp=client.Execute(req);
Console.WriteLine(resp.Content);
//var pp=JsonDocument.Parse(resp.Content);
//Console.WriteLine(pp.RootElement.GetProperty("url"));
}
static bool isCookie(string pp) {
	return pp.IndexOf("#")!=-1;
}
static bool isForm(string pp) {
	return pp.IndexOf("=")!=-1;
}
static bool isHeader(string pp) {
	return pp.IndexOf(":")!=-1;
}
static bool isQuery(string pp) {
	return pp.IndexOf("==")!=-1;
}

/*var c=new Car {
	Name="tomma",
	Wheels=34
};
var a=JsonSerializer.Serialize(c);
Car? j=JsonSerializer.Deserialize<Car>(a);
Console.WriteLine(j?.Name);
class Car {
public string Name {get;set;}="linux";
public int Wheels {get;set;}
}
*/
