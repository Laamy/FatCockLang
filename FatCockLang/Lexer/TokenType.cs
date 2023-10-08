// i want it to support basic stynax like so

/*

import Console from "ConsoleApi"

void foo() 
{
    Console.Write("Hello!");
}

*/

enum TokenType // basic setup for now to support this foo example
{
    Identifier,
    Keyword,
    StringLiteral,
    OpenBrace,
    CloseBrace,
    Import,
    From,
    OpenParenthesis,
    CloseParenthesis
}