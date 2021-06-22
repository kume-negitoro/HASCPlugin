build: HASCPlugin.csproj HASCPlugin.cs
	dotnet build

clean: bin obj
	-rm -rf bin obj
