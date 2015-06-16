# HtmlKit

## What is HtmlKit?

HtmlKit is a cross-platform .NET framework for parsing HTML.

## Goals

I haven't fully figured that out yet.

So far the goal is tokenizing HTML with the intention of using it for
[MimeKit](https://github.com/jstedfast/MimeKit)'s
[HtmlToHtml](http://www.mimekit.net/docs/html/T_MimeKit_Text_HtmlToHtml.htm)
text converter, replacing the quick & dirty HTML tokenizer I originally wrote.

Maybe someday I'll implement a DOM. Who knows.

## License Information

HtmlKit is Copyright (C) 2015 Xamarin Inc. and is licensed under the MIT license:

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.

## Contributing

The first thing you'll need to do is fork HtmlKit to your own GitHub repository. Once you do that,

    git clone git@github.com/<your-account>/HtmlKit.git

If you use [Xamarin Studio](http://xamarin.com/studio) or [MonoDevelop](http://monodevelop.com), all of the
solution files are configured with the coding style used by HtmlKit. If you use Visual Studio or some
other editor, please try to maintain the existing coding style as best as you can.

Once you've got some changes that you'd like to submit upstream to the official HtmlKit repository,
simply send me a Pull Request and I will try to review your changes in a timely manner.

If you'd like to contribute but don't have any particular features in mind to work on, check out the issue
tracker and look for something that might pique your interest!

## Reporting Bugs

Have a bug or a feature request? [Please open a new issue](https://github.com/jstedfast/HtmlKit/issues).

Before opening a new issue, please search for existing issues to avoid submitting duplicates.

## Documentation

API documentation can be found in the source code in the form of XML doc comments.
