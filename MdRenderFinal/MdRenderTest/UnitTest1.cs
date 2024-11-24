using System.Text;
using MdRenderFinal;

namespace MdRenderTest;

public class Tests
{
    private MdRender _mdRender;

    [SetUp]
    public void Setup()
    {
        _mdRender = new MdRender();
    }

    [Test]
    public void Test1()
    {
        StringBuilder sb = new StringBuilder("текст с 1 em подчеркиванием_");
        var result = _mdRender.RenderHtml(sb);
        Assert.AreEqual("текст с 1 em подчеркиванием_", result);
    }

    [Test]
    public void Test2()
    {
        StringBuilder sb = new StringBuilder("текст с 2 em _подчеркиванием_ в начале и конце");
        var result = _mdRender.RenderHtml(sb);
        Assert.AreEqual("текст с 2 em <em>подчеркиванием/<em> в начале и конце", result);
    }

    [Test]
    public void Test3()
    {
        StringBuilder sb = new StringBuilder("текст с 2 em _подчерки_ванием в начале и середине");
        var result = _mdRender.RenderHtml(sb);
        Assert.AreEqual("текст с 2 em <em>подчерки/<em>ванием в начале и середине", result);
    }

    [Test]
    public void Test4()
    {
        StringBuilder sb = new StringBuilder("текст с 2 em подчерки_ванием_ в середине и конце");
        var result = _mdRender.RenderHtml(sb);
        Assert.AreEqual("текст с 2 em подчерки<em>ванием/<em> в середине и конце", result);
    }

    [Test]
    public void Test5()
    {
        StringBuilder sb = new StringBuilder("текст с 1 strong __подчеркиванием");
        var result = _mdRender.RenderHtml(sb);
        Assert.AreEqual("текст с 1 strong __подчеркиванием", result);
    }

    [Test]
    public void Test6()
    {
        StringBuilder sb = new StringBuilder("текст с 2 strong __подчеркиванием__ в начале и конце");
        var result = _mdRender.RenderHtml(sb);
        Assert.AreEqual("текст с 2 strong <strong>подчеркиванием/<strong> в начале и конце", result);
    }

    [Test]
    public void Test7()
    {
        StringBuilder sb = new StringBuilder("текст с 2 strong __подчерки__ванием в начале и середине");
        var result = _mdRender.RenderHtml(sb);
        Assert.AreEqual("текст с 2 strong <strong>подчерки/<strong>ванием в начале и середине", result);
    }

    [Test]
    public void Test8()
    {
        StringBuilder sb = new StringBuilder("текст с 2 strong подчерки__ванием__ в середине и конце");
        var result = _mdRender.RenderHtml(sb);
        Assert.AreEqual("текст с 2 strong подчерки<strong>ванием/<strong> в середине и конце", result);
    }

     [Test]
     public void Test9()
     {
         StringBuilder sb = new StringBuilder("1 экранирование em \\_df_");
         var result = _mdRender.RenderHtml(sb);
         Assert.AreEqual("1 экранирование em _df_", result);
     }

     [Test]
     public void Test10()
     {
         StringBuilder sb = new StringBuilder("2 экранирование em \\\\_df_");
         var result = _mdRender.RenderHtml(sb);
         Assert.AreEqual("2 экранирование em \\<em>df/<em>", result);
     }

     [Test]
     public void Test11()
     {
         StringBuilder sb = new StringBuilder("3 экранирование em \\\\\\_df_");
         var result = _mdRender.RenderHtml(sb);
         Assert.AreEqual("3 экранирование em \\_df_", result);
     }

     [Test]
     public void Test12()
     {
         StringBuilder sb = new StringBuilder("4 экранирование em \\\\\\\\_df_");
         var result = _mdRender.RenderHtml(sb);
         Assert.AreEqual("4 экранирование em \\\\<em>df/<em>", result);
     }

     [Test]
     public void Test13()
     {
         StringBuilder sb = new StringBuilder("1 экранирование strong \\__df__");
         var result = _mdRender.RenderHtml(sb);
         Assert.AreEqual("1 экранирование strong __df__", result);
     }

     [Test]
     public void Test14()
     {
         StringBuilder sb = new StringBuilder("2 экранирование strong \\\\__df__");
         var result = _mdRender.RenderHtml(sb);
         Assert.AreEqual("2 экранирование strong \\<strong>df/<strong>", result);
     }

     [Test]
     public void Test15()
     {
         StringBuilder sb = new StringBuilder("3 экранирование strong \\\\\\__df__");
         var result = _mdRender.RenderHtml(sb);
         Assert.AreEqual("3 экранирование strong \\__df__", result);;
     }

     [Test]
     public void Test16()
     {
         StringBuilder sb = new StringBuilder("4 экранирование strong \\\\\\\\__df__");
         var result = _mdRender.RenderHtml(sb);
         Assert.AreEqual("4 экранирование strong \\\\<strong>df/<strong>", result);
     }


     [Test]
     public void Test17()
     {
         StringBuilder sb = new StringBuilder("_em теги в начале одного слова и в конце другого_");
         var result = _mdRender.RenderHtml(sb);
         Assert.AreEqual("<em>em теги в начале одного слова и в конце другого/<em>", result);
     }

     [Test]
     public void Test18()
     {
         StringBuilder sb = new StringBuilder("__strong теги в начале одного слова и в конце другого__");
         var result = _mdRender.RenderHtml(sb);
         Assert.AreEqual("<strong>strong теги в начале одного слова и в конце другого/<strong>", result);
     }

     [Test]
     public void Test19()
     {
         StringBuilder sb = new StringBuilder("текст с em и цифр_123_ами");
         var result = _mdRender.RenderHtml(sb);
         Assert.AreEqual("текст с em и цифр_123_ами", result);
     }

     [Test]
     public void Test20()
     {
         StringBuilder sb = new StringBuilder("текст с strong и цифр__123__ами");
         var result = _mdRender.RenderHtml(sb);
         Assert.AreEqual("текст с strong и цифр__123__ами", result);
     }

     [Test]
     public void Test21()
     {
         StringBuilder sb = new StringBuilder("# начало абзаца с символом #");
         var result = _mdRender.RenderHtml(sb);
         Assert.AreEqual("<h1> начало абзаца с символом #/<h1>", result);
     }

     [Test]
     public void Test22()
     {
         StringBuilder sb = new StringBuilder("внутри _одинарного __двойное__ не_ работает");
         var result = _mdRender.RenderHtml(sb);
         Assert.AreEqual("внутри <em>одинарного __двойное__ не/<em> работает",result);
     }
     [Test]
     public void Test23()
     {
         StringBuilder sb = new StringBuilder("внутри __двойного выделения _одинарное_ тоже__ работает");
         var result = _mdRender.RenderHtml(sb);
         Assert.AreEqual("внутри <strong>двойного выделения <em>одинарное/<em> тоже/<strong> работает",result);
     }
     [Test]
     public void Test24()
     {
         StringBuilder sb = new StringBuilder("В случае __пересечения _двойных__ и одинарных_ подчерков ни один из них не считается выделением");
         var result = _mdRender.RenderHtml(sb);
         Assert.AreEqual("В случае __пересечения _двойных__ и одинарных_ подчерков ни один из них не считается выделением",result);
     }
     [Test]
     public void Test25()
     {
         string markdownText = @"# Заголовок 1
Это первый абзац. Он кратко вводит тему.
# Подзаголовок 1.1";
         string mdResult = @"<h1> Заголовок 1/<h1>
Это первый абзац. Он кратко вводит тему.
<h1> Подзаголовок 1.1/<h1>";
         StringBuilder sb = new StringBuilder(markdownText);
         var result = _mdRender.RenderHtml(sb);
         Assert.AreEqual(mdResult, result);
     }
}