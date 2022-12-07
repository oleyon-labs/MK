using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAn
{
    public enum TokenTypes
    {
        Id,     //идентификатор
        Num,    //число
        Eq,     //присваивание
        PM,     //+-
        MD,     //*/
        LB,     //(
        RB,     //)
        SC,     //;
        Er,     //ошибка
        EL,     //конец строки
        NT,     //нетерминальный символ
    }
}
