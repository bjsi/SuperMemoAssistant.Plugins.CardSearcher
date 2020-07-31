#region License & Metadata

// The MIT License (MIT)
// 
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// 
// 
// Created On:   7/24/2020 7:01:21 PM
// Modified By:  james

#endregion

using SuperMemoAssistant.Plugins.CardSearcher.CardRenderer;
using SuperMemoAssistant.Plugins.CardSearcher.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;


namespace SuperMemoAssistant.Plugins.CardSearcher.Tests
{

  public class RenderingTests
  {

    private static readonly string collection = @"C:\Users\james\source\repos\AnkiImporter\src\SuperMemoAssistant.Plugins.AnkiImporter.Tests\Fixture\TestCollection\User 1\collection.anki2";
    private static readonly string media = @"C:\Users\james\source\repos\AnkiImporter\src\SuperMemoAssistant.Plugins.AnkiImporter.Tests\Fixture\TestCollection\User 1\collection.media";

    private DataAccess db { get; } = new DataAccess(collection);

    [Fact]
    public async Task RenderCard()
    {

      Expression<Func<Card, bool>> filter = (c) => c.Id == 1518852959231;
      var results = await db.GetCardsAsync(filter).ConfigureAwait(false);
      var card = results.First();

      var q = new Renderer(card, collection, media).Render(TemplateType.Question);
      var a = new Renderer(card, collection, media).Render(TemplateType.Answer);

    }
  }
}
