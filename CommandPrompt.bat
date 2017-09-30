@CD /D "%~dp0"
@title Reposify Command Prompt
@SET PATH=C:\Program Files (x86)\MSBuild\14.0\Bin\;%PATH%
@doskey b=msbuild $* Reposify.proj
type readme.md
%comspec%
