# Native AOT
This sample demonstrates Native AOT compilation. Executing `dotnet publish` enables the deployment of a precompiled application via [Native AOT compilation](https://learn.microsoft.com/dotnet/core/deploying/native-aot/).

## Notes on AOT Compatibility

`Smdn.IO.UsbHid.Abstractions` and `Smdn.IO.UsbHid.Providers.*` themselves and the majority of its dependencies are AOT compatible. However, certain dependencies (including transitive ones) do not yet explicitly declare AOT compatibility (meaning they lack `IsAotCompatible=true` in their assembly metadata), despite their execution paths being implicitly AOT compatible. Consequently, AOT compilation may trigger warnings such as [IL3058](https://learn.microsoft.com/dotnet/core/deploying/native-aot/warnings/il3058), and potential runtime issues (such as unexpected missing metadata or execution failures) cannot be completely ruled out.

For example, `HidApi.Net` (available via `Smdn.IO.UsbHid.Providers.HidApiDotNet`) is the [only backend that declares AOT compatibility](https://github.com/badcel/HidApi.Net#native-aot). However, because a version explicitly marked with `IsAotCompatible=true` has not yet been released, warning IL3058 still occurs during AOT compilation.
