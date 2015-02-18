// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.ComponentModel;
//using System.Data;
using System.IO;

namespace Icu
{ 
    internal enum ErrorCode
    {
        USING_FALLBACK_WARNING = -128,   /** A resource bundle lookup returned a fallback result (not an error) */

        ERROR_WARNING_START = -128,   /** Start of information results (semantically successful) */

        USING_DEFAULT_WARNING = -127,   /** A resource bundle lookup returned a result from the root locale (not an error) */

        SAFECLONE_ALLOCATED_WARNING = -126, /** A SafeClone operation required allocating memory (informational only) */

        STATE_OLD_WARNING = -125,   /** ICU has to use compatibility layer to construct the service. Expect performance/memory usage degradation. Consider upgrading */

        STRING_NOT_TERMINATED_WARNING = -124,/** An output string could not be NUL-terminated because output length==destCapacity. */

        SORT_KEY_TOO_SHORT_WARNING = -123, /** Number of levels requested in getBound is higher than the number of levels in the sort key */

        AMBIGUOUS_ALIAS_WARNING = -122,   /** This converter alias can go to different converter implementations */

        DIFFERENT_UCA_VERSION = -121,     /** ucol_open encountered a mismatch between UCA version and collator image version, so the collator was constructed from rules. No impact to further function */

        ERROR_WARNING_LIMIT,              /** This must always be the last warning value to indicate the limit for UErrorCode warnings (last warning code +1) */


        ZERO_ERROR = 0,     /** No error, no warning. */
        NoErrors = ZERO_ERROR,
        ILLEGAL_ARGUMENT_ERROR = 1,     /** Start of codes indicating failure */
        MISSING_RESOURCE_ERROR = 2,     /** The requested resource cannot be found */
        INVALID_FORMAT_ERROR = 3,     /** Data format is not what is expected */
        FILE_ACCESS_ERROR = 4,     /** The requested file cannot be found */
        INTERNAL_PROGRAM_ERROR = 5,     /** Indicates a bug in the library code */
        MESSAGE_PARSE_ERROR = 6,     /** Unable to parse a message (message format) */
        MEMORY_ALLOCATION_ERROR = 7,     /** Memory allocation error */
        INDEX_OUTOFBOUNDS_ERROR = 8,     /** Trying to access the index that is out of bounds */
        PARSE_ERROR = 9,     /** Equivalent to Java ParseException */
        INVALID_CHAR_FOUND = 10,     /** Character conversion: Unmappable input sequence. In other APIs: Invalid character. */
        TRUNCATED_CHAR_FOUND = 11,     /** Character conversion: Incomplete input sequence. */
        ILLEGAL_CHAR_FOUND = 12,     /** Character conversion: Illegal input sequence/combination of input units. */
        INVALID_TABLE_FORMAT = 13,     /** Conversion table file found, but corrupted */
        INVALID_TABLE_FILE = 14,     /** Conversion table file not found */
        BUFFER_OVERFLOW_ERROR = 15,     /** A result would not fit in the supplied buffer */
        UNSUPPORTED_ERROR = 16,     /** Requested operation not supported in current context */
        RESOURCE_TYPE_MISMATCH = 17,     /** an operation is requested over a resource that does not support it */
        ILLEGAL_ESCAPE_SEQUENCE = 18,     /** ISO-2022 illlegal escape sequence */
        UNSUPPORTED_ESCAPE_SEQUENCE = 19, /** ISO-2022 unsupported escape sequence */
        NO_SPACE_AVAILABLE = 20,     /** No space available for in-buffer expansion for Arabic shaping */
        CE_NOT_FOUND_ERROR = 21,     /** Currently used only while setting variable top, but can be used generally */
        PRIMARY_TOO_LONG_ERROR = 22,     /** User tried to set variable top to a primary that is longer than two bytes */
        STATE_TOO_OLD_ERROR = 23,     /** ICU cannot construct a service from this state, as it is no longer supported */
        TOO_MANY_ALIASES_ERROR = 24,     /** There are too many aliases in the path to the requested resource.
										   It is very possible that a circular alias definition has occured */
        ENUM_OUT_OF_SYNC_ERROR = 25,     /** UEnumeration out of sync with underlying collection */
        INVARIANT_CONVERSION_ERROR = 26,  /** Unable to convert a UChar* string to char* with the invariant converter. */
        INVALID_STATE_ERROR = 27,     /** Requested operation can not be completed with ICU in its current state */
        COLLATOR_VERSION_MISMATCH = 28,   /** Collator version is not compatible with the base version */
        USELESS_COLLATOR_ERROR = 29,     /** Collator is options only and no base is specified */
        NO_WRITE_PERMISSION = 30,     /** Attempt to modify read-only or constant data. */

        STANDARD_ERROR_LIMIT,             /** This must always be the last value to indicate the limit for standard errors */
        /*
         * the error code range 0x10000 0x10100 are reserved for Transliterator
         */
        BAD_VARIABLE_DEFINITION = 0x10000,/** Missing '$' or duplicate variable name */
        PARSE_ERROR_START = 0x10000,    /** Start of Transliterator errors */
        MALFORMED_RULE,                 /** Elements of a rule are misplaced */
        MALFORMED_SET,                  /** A UnicodeSet pattern is invalid*/
        MALFORMED_SYMBOL_REFERENCE,     /** UNUSED as of ICU 2.4 */
        MALFORMED_UNICODE_ESCAPE,       /** A Unicode escape pattern is invalid*/
        MALFORMED_VARIABLE_DEFINITION,  /** A variable definition is invalid */
        MALFORMED_VARIABLE_REFERENCE,   /** A variable reference is invalid */
        MISMATCHED_SEGMENT_DELIMITERS,  /** UNUSED as of ICU 2.4 */
        MISPLACED_ANCHOR_START,         /** A start anchor appears at an illegal position */
        MISPLACED_CURSOR_OFFSET,        /** A cursor offset occurs at an illegal position */
        MISPLACED_QUANTIFIER,           /** A quantifier appears after a segment close delimiter */
        MISSING_OPERATOR,               /** A rule contains no operator */
        MISSING_SEGMENT_CLOSE,          /** UNUSED as of ICU 2.4 */
        MULTIPLE_ANTE_CONTEXTS,         /** More than one ante context */
        MULTIPLE_CURSORS,               /** More than one cursor */
        MULTIPLE_POST_CONTEXTS,         /** More than one post context */
        TRAILING_BACKSLASH,             /** A dangling backslash */
        UNDEFINED_SEGMENT_REFERENCE,    /** A segment reference does not correspond to a defined segment */
        UNDEFINED_VARIABLE,             /** A variable reference does not correspond to a defined variable */
        UNQUOTED_SPECIAL,               /** A special character was not quoted or escaped */
        UNTERMINATED_QUOTE,             /** A closing single quote is missing */
        RULE_MASK_ERROR,                /** A rule is hidden by an earlier more general rule */
        MISPLACED_COMPOUND_FILTER,      /** A compound filter is in an invalid location */
        MULTIPLE_COMPOUND_FILTERS,      /** More than one compound filter */
        INVALID_RBT_SYNTAX,             /** A "::id" rule was passed to the RuleBasedTransliterator parser */
        INVALID_PROPERTY_PATTERN,       /** UNUSED as of ICU 2.4 */
        MALFORMED_PRAGMA,               /** A 'use' pragma is invlalid */
        UNCLOSED_SEGMENT,               /** A closing ')' is missing */
        ILLEGAL_CHAR_IN_SEGMENT,        /** UNUSED as of ICU 2.4 */
        VARIABLE_RANGE_EXHAUSTED,       /** Too many stand-ins generated for the given variable range */
        VARIABLE_RANGE_OVERLAP,         /** The variable range overlaps characters used in rules */
        ILLEGAL_CHARACTER,              /** A special character is outside its allowed context */
        INTERNAL_TRANSLITERATOR_ERROR,  /** Internal transliterator system error */
        INVALID_ID,                     /** A "::id" rule specifies an unknown transliterator */
        INVALID_FUNCTION,               /** A "&amp;fn()" rule specifies an unknown transliterator */
        PARSE_ERROR_LIMIT,              /** The limit for Transliterator errors */
        /*
        * the error code range 0x10100 0x10200 are reserved for formatting API parsing error
        */
        UNEXPECTED_TOKEN = 0x10100,       /** Syntax error in format pattern */
        FMT_PARSE_ERROR_START = 0x10100,  /** Start of format library errors */
        MULTIPLE_DECIMAL_SEPARATORS,    /** More than one decimal separator in number pattern */
        MULTIPLE_EXPONENTIAL_SYMBOLS,   /** More than one exponent symbol in number pattern */
        MALFORMED_EXPONENTIAL_PATTERN,  /** Grouping symbol in exponent pattern */
        MULTIPLE_PERCENT_SYMBOLS,       /** More than one percent symbol in number pattern */
        MULTIPLE_PERMILL_SYMBOLS,       /** More than one permill symbol in number pattern */
        MULTIPLE_PAD_SPECIFIERS,        /** More than one pad symbol in number pattern */
        PATTERN_SYNTAX_ERROR,           /** Syntax error in format pattern */
        ILLEGAL_PAD_POSITION,           /** Pad symbol misplaced in number pattern */
        UNMATCHED_BRACES,               /** Braces do not match in message pattern */
        UNSUPPORTED_PROPERTY,           /** UNUSED as of ICU 2.4 */
        UNSUPPORTED_ATTRIBUTE,          /** UNUSED as of ICU 2.4 */
        ARGUMENT_TYPE_MISMATCH,         /** Argument name and argument index mismatch in MessageFormat functions. */
        DUPLICATE_KEYWORD,              /** Duplicate keyword in PluralFormat. */
        UNDEFINED_KEYWORD,              /** Undefined Plural keyword. */
        DEFAULT_KEYWORD_MISSING,        /** Missing DEFAULT rule in plural rules. */
        FMT_PARSE_ERROR_LIMIT,          /** The limit for format library errors */
        /*
 * the error code range 0x10200 0x102ff are reserved for Break Iterator related error
 */
        BRK_INTERNAL_ERROR = 0x10200,          /** An internal error (bug) was detected.             */
        BRK_ERROR_START = 0x10200,             /** Start of codes indicating Break Iterator failures */
        BRK_HEX_DIGITS_EXPECTED,             /** Hex digits expected as part of a escaped char in a rule. */
        BRK_SEMICOLON_EXPECTED,              /** Missing ';' at the end of a RBBI rule.            */
        BRK_RULE_SYNTAX,                     /** Syntax error in RBBI rule.                        */
        BRK_UNCLOSED_SET,                    /** UnicodeSet witing an RBBI rule missing a closing ']'.  */
        BRK_ASSIGN_ERROR,                    /** Syntax error in RBBI rule assignment statement.   */
        BRK_VARIABLE_REDFINITION,            /** RBBI rule $Variable redefined.                    */
        BRK_MISMATCHED_PAREN,                /** Mis-matched parentheses in an RBBI rule.          */
        BRK_NEW_LINE_IN_QUOTED_STRING,       /** Missing closing quote in an RBBI rule.            */
        BRK_UNDEFINED_VARIABLE,              /** Use of an undefined $Variable in an RBBI rule.    */
        BRK_INIT_ERROR,                      /** Initialization failure.  Probable missing ICU Data. */
        BRK_RULE_EMPTY_SET,                  /** Rule contains an empty Unicode Set.               */
        BRK_UNRECOGNIZED_OPTION,             /** !!option in RBBI rules not recognized.            */
        BRK_MALFORMED_RULE_TAG,              /** The {nnn} tag on a rule is mal formed             */
        BRK_ERROR_LIMIT,                     /** This must always be the last value to indicate the limit for Break Iterator failures */
        /*
 * The error codes in the range 0x10300-0x103ff are reserved for regular expression related errrs
 */
        REGEX_INTERNAL_ERROR = 0x10300,       /** An internal error (bug) was detected.              */
        REGEX_ERROR_START = 0x10300,          /** Start of codes indicating Regexp failures          */
        REGEX_RULE_SYNTAX,                  /** Syntax error in regexp pattern.                    */
        REGEX_INVALID_STATE,                /** RegexMatcher in invalid state for requested operation */
        REGEX_BAD_ESCAPE_SEQUENCE,          /** Unrecognized backslash escape sequence in pattern  */
        REGEX_PROPERTY_SYNTAX,              /** Incorrect Unicode property                         */
        REGEX_UNIMPLEMENTED,                /** Use of regexp feature that is not yet implemented. */
        REGEX_MISMATCHED_PAREN,             /** Incorrectly nested parentheses in regexp pattern.  */
        REGEX_NUMBER_TOO_BIG,               /** Decimal number is too large.                       */
        REGEX_BAD_INTERVAL,                 /** Error in {min,max} interval                        */
        REGEX_MAX_LT_MIN,                   /** In {min,max}, max is less than min.                */
        REGEX_INVALID_BACK_REF,             /** Back-reference to a non-existent capture group.    */
        REGEX_INVALID_FLAG,                 /** Invalid value for match mode flags.                */
        REGEX_LOOK_BEHIND_LIMIT,            /** Look-Behind pattern matches must have a bounded maximum length.    */
        REGEX_SET_CONTAINS_STRING,          /** Regexps cannot have UnicodeSets containing strings.*/
        REGEX_OCTAL_TOO_BIG,                /** Octal character constants must be <= 0377. */
        REGEX_MISSING_CLOSE_BRACKET,        /** Missing closing bracket on a bracket expression. */
        REGEX_INVALID_RANGE,                /** In a character range [x-y], x is greater than y. */
        REGEX_STACK_OVERFLOW,               /** Regular expression backtrack stack overflow. */
        REGEX_TIME_OUT,                     /** Maximum allowed match time exceeded. */
        REGEX_STOPPED_BY_CALLER,            /** Matching operation aborted by user callback fn. */
        REGEX_ERROR_LIMIT,                  /** This must always be the last value to indicate the limit for regexp errors */

        /*
         * The error code in the range 0x10400-0x104ff are reserved for IDNA related error codes
         */
        IDNA_PROHIBITED_ERROR = 0x10400,
        IDNA_ERROR_START = 0x10400,
        IDNA_UNASSIGNED_ERROR,
        IDNA_CHECK_BIDI_ERROR,
        IDNA_STD3_ASCII_RULES_ERROR,
        IDNA_ACE_PREFIX_ERROR,
        IDNA_VERIFICATION_ERROR,
        IDNA_LABEL_TOO_LONG_ERROR,
        IDNA_ZERO_LENGTH_LABEL_ERROR,
        IDNA_DOMAIN_NAME_TOO_LONG_ERROR,
        IDNA_ERROR_LIMIT,
        /*
         * Aliases for StringPrep
         */
        STRINGPREP_PROHIBITED_ERROR = IDNA_PROHIBITED_ERROR,
        STRINGPREP_UNASSIGNED_ERROR = IDNA_UNASSIGNED_ERROR,
        STRINGPREP_CHECK_BIDI_ERROR = IDNA_CHECK_BIDI_ERROR,


        ERROR_LIMIT = IDNA_ERROR_LIMIT      /** This must always be the last value to indicate the limit for UErrorCode (last error code +1) */
    }

    internal class ExceptionFromErrorCode
    {
        public static void ThrowIfError(ErrorCode e)
        {
            ThrowIfError(e, string.Empty, false);
        }

        public static void ThrowIfErrorOrWarning(ErrorCode e)
        {
            ThrowIfError(e, string.Empty, true);
        }

        public static void ThrowIfError(ErrorCode e, string extraInfo)
        {
            ThrowIfError(e, extraInfo, false);
        }

        public static void ThrowIfErrorOrWarning(ErrorCode e, string extraInfo)
        {
            ThrowIfError(e, extraInfo, true);
        }

        private static void ThrowIfError(ErrorCode e, string extraInfo, bool throwOnWarnings)
        {
            switch (e)
            {
                case ErrorCode.ZERO_ERROR: // the only case to not throw!
                    break;
                case ErrorCode.USING_FALLBACK_WARNING:
                    if (throwOnWarnings)
                    {
                        throw new WarningException("Warning: A resource bundle lookup returned a fallback result " + extraInfo);
                    }
                    break;
                case ErrorCode.USING_DEFAULT_WARNING:
                    if (throwOnWarnings)
                    {
                        throw new WarningException("Warning: A resource bundle lookup returned a result from the root locale " + extraInfo);
                    }
                    break;
                case ErrorCode.SAFECLONE_ALLOCATED_WARNING:
                    if (throwOnWarnings)
                    {
                        throw new WarningException("Notice: A SafeClone operation required allocating memory " + extraInfo);
                    }
                    break;
                case ErrorCode.STATE_OLD_WARNING:
                    throw new WarningException("ICU has to use compatibility layer to construct the service. Expect performance/memory usage degradation. Consider upgrading " + extraInfo);
                case ErrorCode.STRING_NOT_TERMINATED_WARNING:
                    throw new WarningException("An output string could not be NUL-terminated because output length==destCapacity. " + extraInfo);
                case ErrorCode.SORT_KEY_TOO_SHORT_WARNING:
                    throw new WarningException("Number of levels requested in getBound is higher than the number of levels in the sort key " + extraInfo);
                case ErrorCode.AMBIGUOUS_ALIAS_WARNING:
                    throw new WarningException("This converter alias can go to different converter implementations " + extraInfo);
                case ErrorCode.DIFFERENT_UCA_VERSION:
                    if (throwOnWarnings)
                    {
                        throw new WarningException(
                                "Warning: ucol_open encountered a mismatch between UCA version and collator image version, so the collator was constructed from rules. No impact to further function " + extraInfo);
                    }
                    break;
                case ErrorCode.ILLEGAL_ARGUMENT_ERROR:
                    throw new ArgumentException(extraInfo);
                case ErrorCode.MISSING_RESOURCE_ERROR:
                    throw new ApplicationException("The requested resource cannot be found " + extraInfo);
                case ErrorCode.INVALID_FORMAT_ERROR:
                    throw new ApplicationException("Data format is not what is expected " + extraInfo);
                case ErrorCode.FILE_ACCESS_ERROR:
                    throw new FileNotFoundException("The requested file cannot be found " + extraInfo);
                case ErrorCode.INTERNAL_PROGRAM_ERROR:
                    throw new InvalidOperationException("Indicates a bug in the library code " + extraInfo);
                case ErrorCode.MESSAGE_PARSE_ERROR:
                    throw new ApplicationException("Unable to parse a message (message format) " + extraInfo);
                case ErrorCode.MEMORY_ALLOCATION_ERROR:
                    throw new OutOfMemoryException(extraInfo);
                case ErrorCode.INDEX_OUTOFBOUNDS_ERROR:
                    throw new IndexOutOfRangeException(extraInfo);
                case ErrorCode.PARSE_ERROR:
                    throw new SyntaxErrorException("Parse Error " + extraInfo);
                case ErrorCode.INVALID_CHAR_FOUND:
                    throw new ArgumentException("Character conversion: Unmappable input sequence. In other APIs: Invalid character. " + extraInfo);
                case ErrorCode.TRUNCATED_CHAR_FOUND:
                    throw new ArgumentException("Character conversion: Incomplete input sequence. " + extraInfo);
                case ErrorCode.ILLEGAL_CHAR_FOUND:
                    throw new ArgumentException("Character conversion: Illegal input sequence/combination of input units. " + extraInfo);
                case ErrorCode.INVALID_TABLE_FORMAT:
                    throw new InvalidDataException("Conversion table file found, but corrupted " + extraInfo);
                case ErrorCode.INVALID_TABLE_FILE:
                    throw new FileNotFoundException("Conversion table file not found " + extraInfo);
                case ErrorCode.BUFFER_OVERFLOW_ERROR:
                    throw new InternalBufferOverflowException("A result would not fit in the supplied buffer " + extraInfo);
                case ErrorCode.UNSUPPORTED_ERROR:
                    throw new InvalidOperationException("Requested operation not supported in current context " + extraInfo);
                case ErrorCode.RESOURCE_TYPE_MISMATCH:
                    throw new InvalidOperationException("an operation is requested over a resource that does not support it " + extraInfo);
                case ErrorCode.ILLEGAL_ESCAPE_SEQUENCE:
                    throw new ArgumentException("ISO-2022 illlegal escape sequence " + extraInfo);
                case ErrorCode.UNSUPPORTED_ESCAPE_SEQUENCE:
                    throw new ArgumentException("ISO-2022 unsupported escape sequence " + extraInfo);
                case ErrorCode.NO_SPACE_AVAILABLE:
                    throw new ArgumentException("No space available for in-buffer expansion for Arabic shaping " + extraInfo);
                case ErrorCode.CE_NOT_FOUND_ERROR:
                    throw new ArgumentException("Collation Element not found " + extraInfo);
                case ErrorCode.PRIMARY_TOO_LONG_ERROR:
                    throw new ArgumentException("User tried to set variable top to a primary that is longer than two bytes " + extraInfo);
                case ErrorCode.STATE_TOO_OLD_ERROR:
                    throw new NotSupportedException("ICU cannot construct a service from this state, as it is no longer supported " + extraInfo);
                case ErrorCode.TOO_MANY_ALIASES_ERROR:
                    throw new ApplicationException("There are too many aliases in the path to the requested resource.\nIt is very possible that a circular alias definition has occured " + extraInfo);
                case ErrorCode.ENUM_OUT_OF_SYNC_ERROR:
                    throw new InvalidOperationException("Enumeration out of sync with underlying collection " + extraInfo);
                case ErrorCode.INVARIANT_CONVERSION_ERROR:
                    throw new ApplicationException("Unable to convert a UChar* string to char* with the invariant converter " + extraInfo);
                case ErrorCode.INVALID_STATE_ERROR:
                    throw new InvalidOperationException("Requested operation can not be completed with ICU in its current state " + extraInfo);
                case ErrorCode.COLLATOR_VERSION_MISMATCH:
                    throw new InvalidOperationException("Collator version is not compatible with the base version " + extraInfo);
                case ErrorCode.USELESS_COLLATOR_ERROR:
                    throw new ApplicationException("Collator is options only and no base is specified " + extraInfo);
                case ErrorCode.NO_WRITE_PERMISSION:
                    throw new ApplicationException("Attempt to modify read-only or constant data. " + extraInfo);
                case ErrorCode.BAD_VARIABLE_DEFINITION:
                    throw new ApplicationException("Transliterator Parse Error: Missing '$' or duplicate variable name " + extraInfo);
                case ErrorCode.MALFORMED_RULE:
                    throw new ApplicationException("Transliterator Parse Error: Elements of a rule are misplaced " + extraInfo);
                case ErrorCode.MALFORMED_SET:
                    throw new ApplicationException("Transliterator Parse Error: A UnicodeSet pattern is invalid " + extraInfo);
                case ErrorCode.MALFORMED_UNICODE_ESCAPE:
                    throw new ApplicationException("Transliterator Parse Error: A Unicode escape pattern is invalid " + extraInfo);
                case ErrorCode.MALFORMED_VARIABLE_DEFINITION:
                    throw new ApplicationException("Transliterator Parse Error: A variable definition is invalid " + extraInfo);
                case ErrorCode.MALFORMED_VARIABLE_REFERENCE:
                    throw new ApplicationException("Transliterator Parse Error: A variable reference is invalid " + extraInfo);
                case ErrorCode.MISPLACED_ANCHOR_START:
                    throw new ApplicationException("Transliterator Parse Error: A start anchor appears at an illegal position " + extraInfo);
                case ErrorCode.MISPLACED_CURSOR_OFFSET:
                    throw new ApplicationException("Transliterator Parse Error: A cursor offset occurs at an illegal position " + extraInfo);
                case ErrorCode.MISPLACED_QUANTIFIER:
                    throw new ApplicationException("Transliterator Parse Error: A quantifier appears after a segment close delimiter " + extraInfo);
                case ErrorCode.MISSING_OPERATOR:
                    throw new ApplicationException("Transliterator Parse Error: A rule contains no operator " + extraInfo);
                case ErrorCode.MULTIPLE_ANTE_CONTEXTS:
                    throw new ApplicationException("Transliterator Parse Error: More than one ante context " + extraInfo);
                case ErrorCode.MULTIPLE_CURSORS:
                    throw new ApplicationException("Transliterator Parse Error: More than one cursor " + extraInfo);
                case ErrorCode.MULTIPLE_POST_CONTEXTS:
                    throw new ApplicationException("Transliterator Parse Error: More than one post context " + extraInfo);
                case ErrorCode.TRAILING_BACKSLASH:
                    throw new ApplicationException("Transliterator Parse Error: A dangling backslash " + extraInfo);
                case ErrorCode.UNDEFINED_SEGMENT_REFERENCE:
                    throw new ApplicationException("Transliterator Parse Error: A segment reference does not correspond to a defined segment " + extraInfo);
                case ErrorCode.UNDEFINED_VARIABLE:
                    throw new ApplicationException("Transliterator Parse Error: A variable reference does not correspond to a defined variable " + extraInfo);
                case ErrorCode.UNQUOTED_SPECIAL:
                    throw new ApplicationException("Transliterator Parse Error: A special character was not quoted or escaped " + extraInfo);
                case ErrorCode.UNTERMINATED_QUOTE:
                    throw new ApplicationException("Transliterator Parse Error: A closing single quote is missing " + extraInfo);
                case ErrorCode.RULE_MASK_ERROR:
                    throw new ApplicationException("Transliterator Parse Error: A rule is hidden by an earlier more general rule " + extraInfo);
                case ErrorCode.MISPLACED_COMPOUND_FILTER:
                    throw new ApplicationException("Transliterator Parse Error: A compound filter is in an invalid location " + extraInfo);
                case ErrorCode.MULTIPLE_COMPOUND_FILTERS:
                    throw new ApplicationException("Transliterator Parse Error: More than one compound filter " + extraInfo);
                case ErrorCode.INVALID_RBT_SYNTAX:
                    throw new ApplicationException("Transliterator Parse Error: A '::id' rule was passed to the RuleBasedTransliterator parser " + extraInfo);
                case ErrorCode.MALFORMED_PRAGMA:
                    throw new ApplicationException("Transliterator Parse Error: A 'use' pragma is invlalid " + extraInfo);
                case ErrorCode.UNCLOSED_SEGMENT:
                    throw new ApplicationException("Transliterator Parse Error: A closing ')' is missing " + extraInfo);
                case ErrorCode.VARIABLE_RANGE_EXHAUSTED:
                    throw new ApplicationException("Transliterator Parse Error: Too many stand-ins generated for the given variable range " + extraInfo);
                case ErrorCode.VARIABLE_RANGE_OVERLAP:
                    throw new ApplicationException("Transliterator Parse Error: The variable range overlaps characters used in rules " + extraInfo);
                case ErrorCode.ILLEGAL_CHARACTER:
                    throw new ApplicationException("Transliterator Parse Error: A special character is outside its allowed context " + extraInfo);
                case ErrorCode.INTERNAL_TRANSLITERATOR_ERROR:
                    throw new ApplicationException("Transliterator Parse Error: Internal transliterator system error " + extraInfo);
                case ErrorCode.INVALID_ID:
                    throw new ApplicationException("Transliterator Parse Error: A '::id' rule specifies an unknown transliterator " + extraInfo);
                case ErrorCode.INVALID_FUNCTION:
                    throw new ApplicationException("Transliterator Parse Error: A '&fn()' rule specifies an unknown transliterator " + extraInfo);
                case ErrorCode.UNEXPECTED_TOKEN:
                    throw new SyntaxErrorException("Format Parse Error: Unexpected token in format pattern " + extraInfo);
                case ErrorCode.MULTIPLE_DECIMAL_SEPARATORS:
                    throw new SyntaxErrorException("Format Parse Error: More than one decimal separator in number pattern " + extraInfo);
                case ErrorCode.MULTIPLE_EXPONENTIAL_SYMBOLS:
                    throw new SyntaxErrorException("Format Parse Error: More than one exponent symbol in number pattern " + extraInfo);
                case ErrorCode.MALFORMED_EXPONENTIAL_PATTERN:
                    throw new SyntaxErrorException("Format Parse Error: Grouping symbol in exponent pattern " + extraInfo);
                case ErrorCode.MULTIPLE_PERCENT_SYMBOLS:
                    throw new SyntaxErrorException("Format Parse Error: More than one percent symbol in number pattern " + extraInfo);
                case ErrorCode.MULTIPLE_PERMILL_SYMBOLS:
                    throw new SyntaxErrorException("Format Parse Error: More than one permill symbol in number pattern " + extraInfo);
                case ErrorCode.MULTIPLE_PAD_SPECIFIERS:
                    throw new SyntaxErrorException("Format Parse Error: More than one pad symbol in number pattern " + extraInfo);
                case ErrorCode.PATTERN_SYNTAX_ERROR:
                    throw new SyntaxErrorException("Format Parse Error: Syntax error in format pattern " + extraInfo);
                case ErrorCode.ILLEGAL_PAD_POSITION:
                    throw new SyntaxErrorException("Format Parse Error: Pad symbol misplaced in number pattern " + extraInfo);
                case ErrorCode.UNMATCHED_BRACES:
                    throw new SyntaxErrorException("Format Parse Error: Braces do not match in message pattern " + extraInfo);
                case ErrorCode.ARGUMENT_TYPE_MISMATCH:
                    throw new SyntaxErrorException("Format Parse Error: Argument name and argument index mismatch in MessageFormat functions. " + extraInfo);
                case ErrorCode.DUPLICATE_KEYWORD:
                    throw new SyntaxErrorException("Format Parse Error: Duplicate keyword in PluralFormat. " + extraInfo);
                case ErrorCode.UNDEFINED_KEYWORD:
                    throw new SyntaxErrorException("Format Parse Error: Undefined Plural keyword. " + extraInfo);
                case ErrorCode.DEFAULT_KEYWORD_MISSING:
                    throw new SyntaxErrorException("Format Parse Error: Missing DEFAULT rule in plural rules. " + extraInfo);
                case ErrorCode.BRK_INTERNAL_ERROR:
                    throw new ApplicationException("Break Error: An internal error (bug) was detected. " + extraInfo);
                case ErrorCode.BRK_HEX_DIGITS_EXPECTED:
                    throw new ApplicationException("Break Error: Hex digits expected as part of a escaped char in a rule. " + extraInfo);
                case ErrorCode.BRK_SEMICOLON_EXPECTED:
                    throw new ApplicationException("Break Error: Missing ';' at the end of a RBBI rule. " + extraInfo);
                case ErrorCode.BRK_RULE_SYNTAX:
                    throw new ApplicationException("Break Error: Syntax error in RBBI rule. " + extraInfo);
                case ErrorCode.BRK_UNCLOSED_SET:
                    throw new ApplicationException("Break Error: UnicodeSet witing an RBBI rule missing a closing ']'. " + extraInfo);
                case ErrorCode.BRK_ASSIGN_ERROR:
                    throw new ApplicationException("Break Error: Syntax error in RBBI rule assignment statement. " + extraInfo);
                case ErrorCode.BRK_VARIABLE_REDFINITION:
                    throw new ApplicationException("Break Error: RBBI rule $Variable redefined. " + extraInfo);
                case ErrorCode.BRK_MISMATCHED_PAREN:
                    throw new ApplicationException("Break Error: Mis-matched parentheses in an RBBI rule. " + extraInfo);
                case ErrorCode.BRK_NEW_LINE_IN_QUOTED_STRING:
                    throw new ApplicationException("Break Error: Missing closing quote in an RBBI rule. " + extraInfo);
                case ErrorCode.BRK_UNDEFINED_VARIABLE:
                    throw new ApplicationException("Break Error: Use of an undefined $Variable in an RBBI rule. " + extraInfo);
                case ErrorCode.BRK_INIT_ERROR:
                    throw new ApplicationException("Break Error: Initialization failure.  Probable missing ICU Data. " + extraInfo);
                case ErrorCode.BRK_RULE_EMPTY_SET:
                    throw new ApplicationException("Break Error: Rule contains an empty Unicode Set. " + extraInfo);
                case ErrorCode.BRK_UNRECOGNIZED_OPTION:
                    throw new ApplicationException("Break Error: !!option in RBBI rules not recognized. " + extraInfo);
                case ErrorCode.BRK_MALFORMED_RULE_TAG:
                    throw new ApplicationException("Break Error: The {nnn} tag on a rule is mal formed " + extraInfo);
                case ErrorCode.BRK_ERROR_LIMIT:
                    throw new ApplicationException("Break Error: This must always be the last value to indicate the limit for Break Iterator failures " + extraInfo);
                case ErrorCode.REGEX_INTERNAL_ERROR:
                    throw new ApplicationException("RegEx Error: An internal error (bug) was detected. " + extraInfo);
                case ErrorCode.REGEX_RULE_SYNTAX:
                    throw new ApplicationException("RegEx Error: Syntax error in regexp pattern. " + extraInfo);
                case ErrorCode.REGEX_INVALID_STATE:
                    throw new ApplicationException("RegEx Error: RegexMatcher in invalid state for requested operation " + extraInfo);
                case ErrorCode.REGEX_BAD_ESCAPE_SEQUENCE:
                    throw new ApplicationException("RegEx Error: Unrecognized backslash escape sequence in pattern " + extraInfo);
                case ErrorCode.REGEX_PROPERTY_SYNTAX:
                    throw new ApplicationException("RegEx Error: Incorrect Unicode property " + extraInfo);
                case ErrorCode.REGEX_UNIMPLEMENTED:
                    throw new ApplicationException("RegEx Error: Use of regexp feature that is not yet implemented. " + extraInfo);
                case ErrorCode.REGEX_MISMATCHED_PAREN:
                    throw new ApplicationException("RegEx Error: Incorrectly nested parentheses in regexp pattern. " + extraInfo);
                case ErrorCode.REGEX_NUMBER_TOO_BIG:
                    throw new ApplicationException("RegEx Error: Decimal number is too large. " + extraInfo);
                case ErrorCode.REGEX_BAD_INTERVAL:
                    throw new ApplicationException("RegEx Error: Error in {min,max} interval " + extraInfo);
                case ErrorCode.REGEX_MAX_LT_MIN:
                    throw new ApplicationException("RegEx Error: In {min,max}, max is less than min. " + extraInfo);
                case ErrorCode.REGEX_INVALID_BACK_REF:
                    throw new ApplicationException("RegEx Error: Back-reference to a non-existent capture group. " + extraInfo);
                case ErrorCode.REGEX_INVALID_FLAG:
                    throw new ApplicationException("RegEx Error: Invalid value for match mode flags. " + extraInfo);
                case ErrorCode.REGEX_LOOK_BEHIND_LIMIT:
                    throw new ApplicationException("RegEx Error: Look-Behind pattern matches must have a bounded maximum length. " + extraInfo);
                case ErrorCode.REGEX_SET_CONTAINS_STRING:
                    throw new ApplicationException("RegEx Error: Regexps cannot have UnicodeSets containing strings. " + extraInfo);
                case ErrorCode.REGEX_OCTAL_TOO_BIG:
                    throw new ApplicationException("Regex Error: Octal character constants must be <= 0377. " + extraInfo);
                case ErrorCode.REGEX_MISSING_CLOSE_BRACKET:
                    throw new ApplicationException("Regex Error: Missing closing bracket on a bracket expression. " + extraInfo);
                case ErrorCode.REGEX_INVALID_RANGE:
                    throw new ApplicationException("Regex Error: In a character range [x-y], x is greater than y. " + extraInfo);
                case ErrorCode.REGEX_STACK_OVERFLOW:
                    throw new ApplicationException("Regex Error: Regular expression backtrack stack overflow. " + extraInfo);
                case ErrorCode.REGEX_TIME_OUT:
                    throw new ApplicationException("Regex Error: Maximum allowed match time exceeded. " + extraInfo);
                case ErrorCode.REGEX_STOPPED_BY_CALLER:
                    throw new ApplicationException("Regex Error: Matching operation aborted by user callback fn. " + extraInfo);
                case ErrorCode.REGEX_ERROR_LIMIT:
                    throw new ApplicationException("RegEx Error:  " + extraInfo);
                case ErrorCode.IDNA_PROHIBITED_ERROR:
                    throw new ApplicationException("IDNA Error: Prohibited " + extraInfo);
                case ErrorCode.IDNA_UNASSIGNED_ERROR:
                    throw new ApplicationException("IDNA Error: Unassigned " + extraInfo);
                case ErrorCode.IDNA_CHECK_BIDI_ERROR:
                    throw new ApplicationException("IDNA Error: Check Bidi " + extraInfo);
                case ErrorCode.IDNA_STD3_ASCII_RULES_ERROR:
                    throw new ApplicationException("IDNA Error: Std3 Ascii Rules Error " + extraInfo);
                case ErrorCode.IDNA_ACE_PREFIX_ERROR:
                    throw new ApplicationException("IDNA Error: Ace Prefix Error " + extraInfo);
                case ErrorCode.IDNA_VERIFICATION_ERROR:
                    throw new ApplicationException("IDNA Error: Verification Error " + extraInfo);
                case ErrorCode.IDNA_LABEL_TOO_LONG_ERROR:
                    throw new ApplicationException("IDNA Error: Label too long " + extraInfo);
                case ErrorCode.IDNA_ZERO_LENGTH_LABEL_ERROR:
                    throw new ApplicationException("IDNA Error: Zero length label " + extraInfo);
                case ErrorCode.IDNA_DOMAIN_NAME_TOO_LONG_ERROR:
                    throw new ApplicationException("IDNA Error: Domain name too long " + extraInfo);
                default:
                    throw new InvalidEnumArgumentException("Missing implementation for ErrorCode " + e);
            }
        }
    }
}
