vs-tool
=======

vs-tool is a Visual Studio 2010 plugin to integrate external compiler/linker toolchains to the VS IDE.

This plugin is a derivation of the excellent work done by Gavin Pugh for the <a href="http://code.google.com/p/vs-android/">vs-android</a> tool, and is therefore also licensed under the <a href="http://en.wikipedia.org/wiki/Zlib_License">Zlib license</a>.

Features
--------
- Seamlessly integrates MinGW (GCC), Clang and Emscripten toolchains to Visual Studio.
- Appears as a new Solution Platform to the Visual Studio projects. Does not destroy or interfere with existing Visual Studio functionality.
- Does not require setting up new solution/project files to target, can be added to existing solutions.
- Adapts the Project Properties dialog to show compiler/linker parameters specific to each tool.

Important! At the current stage, the vs-tool plugin should be considered experimental and hackish in nature. This means that when you update to a newer version of the plugin, solutions that use vs-tool CAN LOSE previously set configuration in the solution/project property pages, or even completely FAIL to load into Visual Studio. You were warned.

If you want to help with the project, please contribute with bug reports and patches. Work on this project is done unpaid, at my own expense, on my free time, if I happen to have a bit of spare time on my hands. If you think vs-tool is crap, or get frustrated that I don't fix your bugs - please go away, I don't want to hear it.

Plugin Installation
-------------------

1. Copy the vs-tool folder from this repository to C:\Program Files (x86)\MSBuild\Microsoft.Cpp\v4.0\Platforms\vs-tool (or the corresponding location where MSBuild exists on your system)
2. To enable an existing solution to be built via vs-tool, create a a platform for it from Configuration Manager -> Active Solution Platform -> New... -> vs-tool.

Setup for MinGW
---------------
MinGW is a toolchain to compile native Windows applications using a Windows port of the GCC compiler. With vs-tool you can build Visual Studio solutions using MinGW.

1. Install MinGW from http://www.mingw.org/
2a. Set MINGW_BIN environment variable to point to the bin\ directory where the MinGW toolchain executables (gcc.exe et al.) reside in.
or
2b. In Visual Studio, go to Project Properties -> Toolchain Directories -> MinGW Compiler Path, and specify there the bin\ directory where the MinGW toolchain executables are located.

Setup for Clang
---------------
<a href="http://clang.llvm.org/">Clang</a> is a C/C++ frontend for the <a href="http://llvm.org/">LLVM Compiler Infrastructure</a>. Vs-tool can also invoke Clang to build Visual Studio solutions. Note however, that Clang does not yet support building native Windows applications.

1. Obtain Clang. For example, follow this web page for instruction how to manually build Clang: http://clang.llvm.org/get_started.html .
2a. Set CLANG_BIN environment variable to point to the directory where the Clang toolchain executables (clang.exe and the rest) reside in.
or 
2b. In Visual Studio, go to Project Properties -> Toolchain Directories -> Clang Compiler Path, and specify there the directory where the Clang toolchain executables are located.

Setup for Emscripten (emcc)
--------------
Emscripten is a compiler/linker that allows you to compile C/C++ code to JavaScript. See http://emscripten.org/ . Vs-tool can be used to build Visual Studio solutions to JavaScript.

1. Follow these instructions to set up Emscripten: https://github.com/kripken/emscripten/wiki/Tutorial .
2. Emscripten requires Clang to be set up. Follow the above instructions to set up Clang for vs-tool.
3. Copy emcc\emccstub.exe into the folder where you cloned Emscripten, so that emccstub.exe resides in the same folder than the file emcc.
4a. Set EMCC_BIN environment variable to point to the directory where the file emcc and emccstub.exe are located in.
or 
4b. In Visual Studio, go to Project Properties -> Toolchain Directories -> Emcc Linker Path, and specify there the directory where the files emcc and emccstub.exe are located in.
