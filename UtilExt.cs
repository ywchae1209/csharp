
using LanguageExt;
using static LanguageExt.Prelude;

static public class UtilExt
{
    // lift Action(void returning function) ==> Unit
    //------------------------------------------------------------
    
    // void --> void :: void --> Unit
    public static Func<Unit> lift(Action f) => () =>
    {
        f();
        return unit;
    };

    // A --> void :: A --> Unit
    public static Func<A, Unit> lift<A>(Action<A> f) => a =>
    {
        f(a);
        return unit;
    };

    // Option ext.
    //------------------------------------------------------------
    
    // Option<R> --> Either<L, R>
    public static Either<L, R> ToRight<L, R>(this Option<R> o, L l)
        => o.Match(
            r => Either<L,R>.Right(r),
            Left(l) );
    
    // Option<L> --> Either<L, R>
    public static Either<L, R> ToLeft<L, R>(this Option<L> o, R l)
        => o.Match(
            l => Either<L,R>.Left(l),
            Right(l) );
    

    // Numeric ext
    //------------------------------------------------------------
    // string --> Option<int>
    public static Option<int> ToInt(this string st)
        => int.TryParse(st, out var r) ? Some(r) : None;

    // string --> Option<long>
    public static Option<long> ToLong(this string st)
        => long.TryParse(st, out var r) ? Some(r) : None;

    // string --> Option<double>
    public static Option<double> ToDouble(this string st)
        => double.TryParse(st, out var r) ? Some(r) : None;

    // string --> Option<float>
    public static Option<float> ToFloat(this string st)
        => float.TryParse(st, out var r) ? Some(r) : None;

    
    // IEnumerable ext
    //------------------------------------------------------------
    
    // Foreach :: do side effects
    public static IEnumerable<Unit> Foreach<A>(this IEnumerable<A> es, Action<A> f)
        => es.Map(lift(f));
    
    // MkString 
    public static string MkString(this IEnumerable<string> es, string sep)
        => string.Join(sep, es);

    // MkString 
    public static string MkString(this IEnumerable<string> es, string start, string sep, string end)
        => string.Concat(start, string.Join(sep, es), end);

    // Function ext.
    //------------------------------------------------------------
    public static Func<A,C> AndThen<A, B, C>(this Func<A, B> f, Func<B, C> g)
        => a => g(f(a));

    // peeking :: ex) logging
    public static Func<A,A> tap<A>(Action<A> peek) => a => {
            peek(a);
            return a;
        };

    // Collection ext.
    //------------------------------------------------------------
    public static Option<T> Get<T>(this Lst<T> l, int idx)
        => l[idx] ?? Option<T>.None;


    public static Option<T> Get<K, T>(this Map<K,T> l, K key)
        => Optional(key).Bind(k => l[k] ?? Option<T>.None);

    // Lift functions for exception-handling :: try-catch ==> Option
    //------------------------------------------------------------

    // may(f) ::::
    // void --> A  ===>  void --> Option<A>
    // void --> void ===> void --> Option<Unit>
    // A --> B  ===> A --> Option<B>
    // A --> void ===> A --> Option<Unit>
    public static Func<Option<A>> may<A>(Func<A> f) => () =>
    {
        try
        {
            return f();
        } 
        catch (Exception e)
        {
            return Option<A>.None;
        }
    };
    
    public static Func<Option<Unit>> may(Action f) => may(() =>
    {
        f();
        return unit;
    });

    public static Func<A, Option<B>> may<A, B>(Func<A, B> f) 
        => a => may(() => f(a))();

    public static Func<A, Option<Unit>> may<A>(Action<A> f)
        => a => may(() => f(a))();

    //------------------------------------------------------------
    public record Err(string Reason);
    
    // mayOr(f) ::::
    // void --> A  ===>  void --> Either<Error, A>
    // void --> void ===> void --> Either<Error, Unit>
    // A --> B  ===> A --> Either<Error, B>
    // A --> void ===> A --> Either<Error, Unit>

    public static Func<Either<Err, A>> mayOr<A>(Func<A> f) => () =>
    {
        try
        {
            return Optional(f()).ToRight(new Err("null"));
        }
        catch (Exception e)
        {
            return Left(new Err(e.ToString()));
        }
    };

    public static Func<Either<Err, Unit>> mayOr(Action f) 
        => mayOr(
            () =>
            {
                f();
                return unit;
            });

    public static Func<A, Either<Err, B>> mayOr<A, B>(Func<A, B> f)
        => a => mayOr(() => f(a))();

    public static Func<A, Either<Err, Unit>> mayOr<A>(Action<A> f)
        => a => mayOr(() => f(a))();

    //------------------------------------------------------------

}
