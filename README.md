# CalculatorApp
A Console App for addition, subtraction, multiplication, and division operations

## Getting Started

### Run without IDE
```
dotnet publish -c Debug -r win10-x64
..\[Solution Folder]\CaculatorApp\bin\Debug\netcoreapp2.2\CalculatorApp.exe  /D 1 /A abc /U 200
```
or
```
..\[Solution Folder]\CaculatorApp>dotnet build
..\[Solution Folder]\CaculatorApp>dotnet run /D 1 /A abc /U 200
```

### Arguments
```
/D 1    // set deny negative numbers, 1 or true, 0 or false, optional, default is true
/A abc  // set alternate delimiter,  optional, default is "\n" ("\" already escaped)
/U 200  // set number upper bound, optional, default is 1000
```

### Example of inputs and outputs


```
Input>
1*2
0 = 0
Input>
1,2
1+2 = 3
Input>
4.5
0 = 0
Input>
//a\n1a1
1+1 = 2
Input>
//a\n1a1,4,6,7
1+1+4+6+7 = 19
Input>
//+[+]-[-]*[*]/[/]\n1*3-5+6/3*4
1*3-5+6/3*4 = 6
Input>
//+[a]-[b]*[cc]/[!!]\n1cc3b5a6!!3cc4
1*3-5+6/3*4 = 6
Input>
//[!!][***]\n2,4!!***4
2+4+0+4 = 10
Input>
//[!!][***]\n2,4!!5***4
2+4+5+4 = 15
Input>
//[  ][a]\n2,4  5,4
2+4+5+4 = 15
Input>
2,2000
2+0 = 2
Input>
3,ffr,454,65
3+0+454+65 = 522
Input>
1,2,-3
Negative number(s) denied: -3
Input>
//[!!][!!!]\n2,4!!5***4
Overlapping Delimiters denied: !!!, !!
Input>
```
