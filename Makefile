build: HASCPlugin.csproj HASCPlugin.cs DataRowCreator.cs HASCDataRow.cs
	dotnet build

clean: bin obj
	-rm -rf bin obj
