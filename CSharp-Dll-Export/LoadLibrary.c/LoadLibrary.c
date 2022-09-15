#define PathToLibrary "E:\\MyCode\\BlogCodes\\CSharp-Dll-Export\\CSharpDllExport\\CSharpDllExport\\bin\\Release\\net7.0\\win-x64\\publish\\CSharpDllExport.dll"

// 导入必要的头文件
#include <windows.h>
#include <stdlib.h>
#include <stdio.h>

int callAddFunc(char* path, char* funcName, int a, int b);
char* callConcatStringFunc(char* path, char* funcName, char* firstString, char* secondString);

int main()
{
    // 检查文件是否存在
    if (access(PathToLibrary, 0) == -1)
    {
        puts("没有在指定的路径找到库文件");
        return 0;
    }

    // 计算两个值的和
    int sum = callAddFunc(PathToLibrary, "Add", 2, 8);
    printf("两个值的和是 %d \n", sum);

    // 拼接两个字符串
    char* result = callConcatStringFunc(PathToLibrary, "ConcatString", ".NET", " yyds");
    printf("拼接字符串的结果为 %s \n", result);
}

int callAddFunc(char* path, char* funcName, int firstInt, int secondInt)
{
    // 调用 C# 共享库的函数来计算两个数的和
    HINSTANCE handle = LoadLibraryA(path);

    typedef int(*myFunc)(int, int);
    myFunc MyImport = (myFunc)GetProcAddress(handle, funcName);

    int result = MyImport(firstInt, secondInt);

    return result;
}

char* callConcatStringFunc(char* path, char* funcName, char* firstString, char* secondString)
{

    HINSTANCE handle = LoadLibraryA(path);
    typedef char* (*myFunc)(char*, char*);

    myFunc MyImport = (myFunc)GetProcAddress(handle, funcName);

    // 传递指针并且返回指针
    char* result = MyImport(firstString, secondString);

    return result;
}