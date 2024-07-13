
using static System.Console;
using static LanguageExt.Prelude;
using LanguageExt;
using static Second.UtilExt;

static class Program
{
    public static void Main(string[] args)
    {
        
        Func<int, Func<int, int>> add =  x => y => x + y;
        Func<int, int> add3 = add(3);

        //------------------------------------------------------------------------------
        // Exercise : Option
        //------------------------------------------------------------------------------

        var o = Some(1);
        var o0 = o.ToList();                  // convert to list
        var o1 = o.Map(add3);
        var o2 = o.Fold(0, (_, a) => a);       // getOrElse(0)
        var o3 = o.IfNone(0);                  // getOrElse(0)
        var o4 = o.Match(                      // 
            Some: v => v.ToString(),
            ""
        );

        Func<int> other = () => 1;
        var o5 = o.IfNone(() => other());       // lazy alternative ??
        var o6 = o.IfSome(WriteLine);           // foreach in scala

        var o7 = Optional((string)null);                   // null check :: to None


        Option<int> someOption0(int x) => (x == 1)? 1 : None;          // implicit conversion 
        Option<int> someOption1(int x) => (x == 1)? Some(1) : None;    // explicit version

        var oo1 = Some(1);
        var oo2 = Some(1);
        var oo3 = Some(1);
        var oo4 = Some(1);

        var map4 =
            from x1 in oo1
            from x2 in oo2
            from x3 in oo3
            from x4 in oo4
            select (x1 + x2 + x3 + x4);

        var flat4 = oo1.Bind(x1 => oo2.Bind( x2 => oo3.Bind( x3 => oo4.Map( x4 => x1 + x2 + x3 + x4 ))));

        //------------------------------------------------------------------------------
        // Exercise : OptionT :: Monad<Option<T>> 
        //------------------------------------------------------------------------------

        var lo = List(Some(1), None, Some(2));
        var lm0 = lo.MapT( i => i + 1);
        var lm1 = lo.BindT( i => Some(i + 1));

        Option<int> Foo( Some<string> arg)         // arg. null checking.
        {
            string value = arg.Value;              // extract value
            return (value == "") ? 1 : None;
        }



        //------------------------------------------------------------------------------
        // Exercise : using... Unit
        //------------------------------------------------------------------------------

        Unit Empty()
        {
            return unit;
        }

        var wl = lift<int>(WriteLine);
        var wl0 = List(1, 2, 3).Map(wl);
        
        var np = List(1, 2, 3)
            .Map(tap<int>(WriteLine))       // custom extension
            .Map(lift<int>(WriteLine));     // custom extension

        var notImplicitPromoted = List(1, 2, 3).Map(i =>
        {
            WriteLine(i);
            return unit;                // void not-implicitly converted to Unit
        }).Map(i =>
        {
            WriteLine(i.ToString());    // () will printed
            return unit;                // void not-implicitly converted to Unit
        });

    //------------------------------------------------------------------------------
    // Exercise : immutable collections
    //------------------------------------------------------------------------------

        var l = List(1, 2, 3);
        var l0 = l.Map(i => i * 10)
            .Filter( i => i > 5)
            .TakeWhile( i => i < 100)
            .HeadOrNone();                             // headOption

        var l1 = l.Fold("", (s, a) => s + $": {a}");   // fold left
        var l2 = l.Find(i => i == 3);
        var l3 = l.Contains(2);
        var l4 = l.ForAll(i => i > 100);
        var l5 = l.Any(i => i > 100);
        var l6 = l.Filter(i => i == 4);
        var l7 = l.Get(23);                            // custom extension
        var l8 = l.SetItem(23, 1);                     // new Lst, may cause exception

        int lf(IEnumerable<int> ii)                    // pattern matching
            => match(
                ii,
                () => 0,
                (x, xs) => x + lf(xs)
            );

        var l2m = l
            .Map(i => (i.ToString(), i))
            .ToMap();
        
        var l2m0 = l2m.AddOrUpdate("", 1);
        var l2m1 = l2m.Get("");                     // custom extension
        
        var lg = l
            .Map(i => (i.ToString(), i))
            .GroupBy(kv => kv.Item1 );                        // todo :: need check

        var m = Map(
            ("1", 1),
            ("2", 1),
            ("3", 1),
            ("4", 1)
        );

        var fe = Range(1, 100).Foreach(WriteLine);

        var f1 = (int i) => i.ToString();
        var f2 = (string s) => s.Length;
        var f3 = f1.AndThen(f2);

        var z = new Exception("");

    }
}
