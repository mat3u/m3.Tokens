m3.Tokens
=========

User friendly (relatively), fast unique token generation library.

Eg.: for anonymous access scenarios.

Token structure (defaults):

   * before 2026-02-01 04:51:33: xxxxx-xxxxx-xxxxx (5-5-5)
   * before 2080-11-19 03:14:07: xxxxxx-xxxxx-xxxxx (6-5-5)

after this date Int32 variable which holds date related part will end, and trying to generate new token will cause an Exception.
Those dates can be changed by using different epoch start (default: 2012-11-01).


