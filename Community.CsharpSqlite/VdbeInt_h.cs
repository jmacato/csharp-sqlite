using System;
using FILE = System.IO.TextWriter;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using unsigned = System.UIntPtr;
using Pgno = System.UInt32;
#if !SQLITE_MAX_VARIABLE_NUMBER
using ynVar = System.Int16;

#else
using ynVar = System.Int32;
#endif

namespace Community.CsharpSqlite
{
    using Op = Sqlite3.VdbeOp;

    public partial class Sqlite3
    {
        /*
        ** 2003 September 6
        **
        ** The author disclaims copyright to this source code.  In place of
        ** a legal notice, here is a blessing:
        **
        **    May you do good and not evil.
        **    May you find forgiveness for yourself and forgive others.
        **    May you share freely, never taking more than you give.
        **
        *************************************************************************
        ** This is the header file for information that is private to the
        ** VDBE.  This information used to all be at the top of the single
        ** source code file "vdbe.c".  When that file became too big (over
        ** 6000 lines long) it was split up into several smaller files and
        ** this header information was factored out.
        *************************************************************************
        **  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
        **  C#-SQLite is an independent reimplementation of the SQLite software library
        **
        **  SQLITE_SOURCE_ID: 2009-12-07 16:39:13 1ed88e9d01e9eda5cbc622e7614277f29bcc551c
        **
        **  $Header$
        *************************************************************************
        */
        //#if !_VDBEINT_H_
        //#define _VDBEINT_H_

        /*
        ** SQL is translated into a sequence of instructions to be
        ** executed by a virtual machine.  Each instruction is an instance
        ** of the following structure.
        */
        //typedef struct VdbeOp Op;

        /*
        ** Boolean values
        */
        //typedef unsigned char Bool;

        /*
        ** A cursor is a pointer into a single BTree within a database file.
        ** The cursor can seek to a BTree entry with a particular key, or
        ** loop over all entries of the Btree.  You can also insert new BTree
        ** entries or retrieve the key or data from the entry that the cursor
        ** is currently pointing to.
        **
        ** Every cursor that the virtual machine has open is represented by an
        ** instance of the following structure.
        **
        ** If the VdbeCursor.isTriggerRow flag is set it means that this cursor is
        ** really a single row that represents the NEW or OLD pseudo-table of
        ** a row trigger.  The data for the row is stored in VdbeCursor.pData and
        ** the rowid is in VdbeCursor.iKey.
        */
        public class VdbeCursor
        {
            public BtCursor pCursor; /* The cursor structure of the backend */
            public int iDb; /* Index of cursor database in db.aDb[] (or -1) */
            public long lastRowid; /* Last rowid from a Next or NextIdx operation */
            public bool zeroed; /* True if zeroed out and ready for reuse */
            public bool rowidIsValid; /* True if lastRowid is valid */
            public bool atFirst; /* True if pointing to first entry */
            public bool useRandomRowid; /* Generate new record numbers semi-randomly */
            public bool nullRow; /* True if pointing to a row with no data */
            public bool deferredMoveto; /* A call to sqlite3BtreeMoveto() is needed */
            public bool isTable; /* True if a table requiring integer keys */
            public bool isIndex; /* True if an index containing keys only - no data */
            public long movetoTarget; /* Argument to the deferred sqlite3BtreeMoveto() */
            public Btree pBt; /* Separate file holding temporary table */
            public int pseudoTableReg; /* Register holding pseudotable content. */
            public KeyInfo pKeyInfo; /* Info about index keys needed by index cursors */
            public int nField; /* Number of fields in the header */
            public int seqCount; /* Sequence counter */
#if !SQLITE_OMIT_VIRTUALTABLE
public sqlite3_vtab_cursor pVtabCursor;  /* The cursor for a virtual table */
public readonly sqlite3_module pModule; /* Module for cursor pVtabCursor */
#endif

            /* Result of last sqlite3BtreeMoveto() done by an OP_NotExists or
            ** OP_IsUnique opcode on this cursor. */
            public int seekResult;

            /* Cached information about the header for the data record that the
            ** cursor is currently pointing to.  Only valid if cacheStatus matches
            ** Vdbe.cacheCtr.  Vdbe.cacheCtr will never take on the value of
            ** CACHE_STALE and so setting cacheStatus=CACHE_STALE guarantees that
            ** the cache is out of date.
            **
            ** aRow might point to (ephemeral) data for the current row, or it might
            ** be NULL.
            */
            public uint cacheStatus; /* Cache is valid if this matches Vdbe.cacheCtr */
            public uint payloadSize; /* Total number of bytes in the record */
            public uint[] aType; /* Type values for all entries in the record */
            public uint[] aOffset; /* Cached offsets to the start of each columns data */
            public int aRow; /* Pointer to Data for the current row, if all on one page */

            public VdbeCursor Copy()
            {
                return (VdbeCursor)MemberwiseClone();
            }
        }
        //typedef struct VdbeCursor VdbeCursor;


        /*
        ** When a sub-program is executed (OP_Program), a structure of this type
        ** is allocated to store the current value of the program counter, as
        ** well as the current memory cell array and various other frame specific
        ** values stored in the Vdbe struct. When the sub-program is finished, 
        ** these values are copied back to the Vdbe from the VdbeFrame structure,
        ** restoring the state of the VM to as it was before the sub-program
        ** began executing.
        **
        ** Frames are stored in a linked list headed at Vdbe.pParent. Vdbe.pParent
        ** is the parent of the current frame, or zero if the current frame
        ** is the main Vdbe program.
        */
        //typedef struct VdbeFrame VdbeFrame;
        public class VdbeFrame
        {
            public VdbeCursor[] aChildCsr; /* Array of cursors for child frame */

            //
            // Needed for C# Implementation
            //
            public Mem[] aChildMem; /* Array of memory cells for child frame */
            public Mem[] aMem; /* Array of memory cells */
            public Op[] aOp; /* Program instructions */
            public VdbeCursor[] apCsr; /* Element of Vdbe cursors */
            public long lastRowid; /* Last insert rowid (sqlite3.lastRowid) */
            public int nChange; /* Statement changes (Vdbe.nChanges)     */
            public int nChildCsr; /* Number of cursors for child frame */
            public int nChildMem; /* Number of memory cells for child frame */
            public ushort nCursor; /* Number of entries in apCsr */
            public int nMem; /* Number of entries in aMem */
            public int nOp; /* Size of aOp array */
            public int pc; /* Program Counter */
            public VdbeFrame pParent; /* Parent of this frame */
            public int token; /* Copy of SubProgram.token */
            public Vdbe v; /* VM this frame belongs to */
        }

        //#define VdbeFrameMem(p) ((Mem *)&((u8 *)p)[ROUND8(sizeof(VdbeFrame))])
        /*
        ** A value for VdbeCursor.cacheValid that means the cache is always invalid.
        */
        private const int CACHE_STALE = 0;

        /*
        ** Internally, the vdbe manipulates nearly all SQL values as Mem
        ** structures. Each Mem struct may cache multiple representations (string,
        ** integer etc.) of the same value.  A value (and therefore Mem structure)
        ** has the following properties:
        **
        ** Each value has a manifest type. The manifest type of the value stored
        ** in a Mem struct is returned by the MemType(Mem*) macro. The type is
        ** one of SQLITE_NULL, SQLITE_INTEGER, SQLITE_REAL, SQLITE_TEXT or
        ** SQLITE_BLOB.
        */
        public class Mem
        {
            public struct union_ip
            {
#if DEBUG_CLASS_MEM || DEBUG_CLASS_ALL
public i64 _i;              /* First operand */
public i64 i
{
get { return _i; }
set { _i = value; }
}
#else
                public long i; /* Integer value. */
#endif
                public int nZero; /* Used when bit MEM_Zero is set in flags */
                public FuncDef pDef; /* Used only when flags==MEM_Agg */
                public RowSet pRowSet; /* Used only when flags==MEM_RowSet */
                public VdbeFrame pFrame; /* Used when flags==MEM_Frame */
            }

            public union_ip u;
            public double r; /* Real value */
            public sqlite3 db; /* The associated database connection */
            public string z; /* String value */
            public byte[] zBLOB; /* BLOB value */
            public int n; /* Number of characters in string value, excluding '\0' */
#if DEBUG_CLASS_MEM || DEBUG_CLASS_ALL
public u16 _flags;              /* First operand */
public u16 flags
{
get { return _flags; }
set { _flags = value; }
}
#else
            public ushort flags = MEM_Null; /* Some combination of MEM_Null, MEM_Str, MEM_Dyn, etc. */
#endif
            public byte type = SQLITE_NULL; /* One of SQLITE_NULL, SQLITE_TEXT, SQLITE_INTEGER, etc */
            public byte enc; /* SQLITE_UTF8, SQLITE_UTF16BE, SQLITE_UTF16LE */

            public dxDel xDel; /* If not null, call this function to delete Mem.z */

            // Not used under c#
            //public string zMalloc;      /* Dynamic buffer allocated by sqlite3Malloc() */
            public Mem _Mem; /* Used when C# overload Z as MEM space */
            public SumCtx _SumCtx; /* Used when C# overload Z as Sum context */
            public SubProgram[] _SubProgram; /* Used when C# overload Z as SubProgram*/
            public StrAccum _StrAccum; /* Used when C# overload Z as STR context */
            public object _MD5Context; /* Used when C# overload Z as MD5 context */


            public void CopyTo(Mem ct)
            {
                ct.u = u;
                ct.r = r;
                ct.db = db;
                ct.z = z;
                if (zBLOB == null)
                {
                    ct.zBLOB = null;
                }
                else
                {
                    ct.zBLOB = sqlite3Malloc(zBLOB.Length);
                    Buffer.BlockCopy(zBLOB, 0, ct.zBLOB, 0, zBLOB.Length);
                }

                ct.n = n;
                ct.flags = flags;
                ct.type = type;
                ct.enc = enc;
                ct.xDel = xDel;
            }
        }

        /* One or more of the following flags are set to indicate the validOK
        ** representations of the value stored in the Mem struct.
        **
        ** If the MEM_Null flag is set, then the value is an SQL NULL value.
        ** No other flags may be set in this case.
        **
        ** If the MEM_Str flag is set then Mem.z points at a string representation.
        ** Usually this is encoded in the same unicode encoding as the main
        ** database (see below for exceptions). If the MEM_Term flag is also
        ** set, then the string is nul terminated. The MEM_Int and MEM_Real
        ** flags may coexist with the MEM_Str flag.
        **
        ** Multiple of these values can appear in Mem.flags.  But only one
        ** at a time can appear in Mem.type.
        */
        //#define MEM_Null      0x0001   /* Value is NULL */
        //#define MEM_Str       0x0002   /* Value is a string */
        //#define MEM_Int       0x0004   /* Value is an integer */
        //#define MEM_Real      0x0008   /* Value is a real number */
        //#define MEM_Blob      0x0010   /* Value is a BLOB */
        //#define MEM_RowSet    0x0020   /* Value is a RowSet object */
        //#define MEM_Frame     0x0040   /* Value is a VdbeFrame object */
        //#define MEM_TypeMask  0x00ff   /* Mask of type bits */
        private const int MEM_Null = 0x0001;
        private const int MEM_Str = 0x0002;
        private const int MEM_Int = 0x0004;
        private const int MEM_Real = 0x0008;
        private const int MEM_Blob = 0x0010;
        private const int MEM_RowSet = 0x0020;
        private const int MEM_Frame = 0x0040;
        private const int MEM_TypeMask = 0x00ff;

        /* Whenever Mem contains a valid string or blob representation, one of
        ** the following flags must be set to determine the memory management
        ** policy for Mem.z.  The MEM_Term flag tells us whether or not the
        ** string is \000 or \u0000 terminated
        //    */
        //#define MEM_Term      0x0200   /* String rep is nul terminated */
        //#define MEM_Dyn       0x0400   /* Need to call sqliteFree() on Mem.z */
        //#define MEM_Static    0x0800   /* Mem.z points to a static string */
        //#define MEM_Ephem     0x1000   /* Mem.z points to an ephemeral string */
        //#define MEM_Agg       0x2000   /* Mem.z points to an agg function context */
        //#define MEM_Zero      0x4000   /* Mem.i contains count of 0s appended to blob */
        //#ifdef SQLITE_OMIT_INCRBLOB
        //  #undef MEM_Zero
        //  #define MEM_Zero 0x0000
        //#endif
        private const int MEM_Term = 0x0200;
        private const int MEM_Dyn = 0x0400;
        private const int MEM_Static = 0x0800;
        private const int MEM_Ephem = 0x1000;
        private const int MEM_Agg = 0x2000;
#if !SQLITE_OMIT_INCRBLOB
    const int MEM_Zero = 0x4000;
#else
        private const int MEM_Zero = 0x0000;
#endif

        /*
        ** Clear any existing type flags from a Mem and replace them with f
        */
        //#define MemSetTypeFlag(p, f) \
        //   ((p)->flags = ((p)->flags&~(MEM_TypeMask|MEM_Zero))|f)
        private static void MemSetTypeFlag(Mem p, int f)
        {
            p.flags = (ushort)((p.flags & ~(MEM_TypeMask | MEM_Zero)) | f);
        } // TODO -- Convert back to inline for speed

#if SQLITE_OMIT_INCRBLOB
        //#undef MEM_Zero
#endif

        /* A VdbeFunc is just a FuncDef (defined in sqliteInt.h) that contains
    ** additional information about auxiliary information bound to arguments
    ** of the function.  This is used to implement the sqlite3_get_auxdata()
    ** and sqlite3_set_auxdata() APIs.  The "auxdata" is some auxiliary data
    ** that can be associated with a constant argument to a function.  This
    ** allows functions such as "regexp" to compile their constant regular
    ** expression argument once and reused the compiled code for multiple
    ** invocations.
    */
        public class AuxData
        {
            public string pAux; /* Aux data for the i-th argument */
            public dxDel xDelete; //(void *);      /* Destructor for the aux data */
        }

        public class VdbeFunc : FuncDef
        {
            public AuxData[] apAux = new AuxData[2]; /* One slot for each function argument */
            public int nAux; /* Number of entries allocated for apAux[] */
            public FuncDef pFunc; /* The definition of the function */
        }

        /*
        ** The "context" argument for a installable function.  A pointer to an
        ** instance of this structure is the first argument to the routines used
        ** implement the SQL functions.
        **
        ** There is a typedef for this structure in sqlite.h.  So all routines,
        ** even the public interface to SQLite, can use a pointer to this structure.
        ** But this file is the only place where the internal details of this
        ** structure are known.
        **
        ** This structure is defined inside of vdbeInt.h because it uses substructures
        ** (Mem) which are only defined there.
        */
        public class sqlite3_context
        {
            public int isError; /* Error code returned by the function. */
            public CollSeq pColl; /* Collating sequence */
            public FuncDef pFunc; /* Pointer to function information.  MUST BE FIRST */
            public Mem pMem; /* Memory cell used to store aggregate context */
            public VdbeFunc pVdbeFunc; /* Auxilary data, if created. */
            public Mem s = new Mem(); /* The return value is stored here */
        }

        /*
        ** A Set structure is used for quick testing to see if a value
        ** is part of a small set.  Sets are used to implement code like
        ** this:
        **            x.y IN ('hi','hoo','hum')
        */
        //typedef struct Set Set;
        public class Set
        {
            private Hash hash; /* A set is just a hash table */
            private HashElem prev; /* Previously accessed hash elemen */
        }

        /*
        ** An instance of the virtual machine.  This structure contains the complete
        ** state of the virtual machine.
        **
        ** The "sqlite3_stmt" structure pointer that is returned by sqlite3_compile()
        ** is really a pointer to an instance of this structure.
        **
        ** The Vdbe.inVtabMethod variable is set to non-zero for the duration of
        ** any virtual table method invocations made by the vdbe program. It is
        ** set to 2 for xDestroy method calls and 1 for all other methods. This
        ** variable is used for two purposes: to allow xDestroy methods to execute
        ** "DROP TABLE" statements and to prevent some nasty side effects of
        ** malloc failure when SQLite is invoked recursively by a virtual table
        ** method function.
        */
        public class Vdbe
        {
            public Mem[] aColName; /* Column names to return */
            public int[] aCounter = new int[2]; /* Counters used by sqlite3_stmt_status() */
            public int[] aLabel; /* Space to hold the labels */
            public Mem[] aMem; /* The memory locations */
            public BtreeMutexArray aMutex; /* An array of Btree used here and needing locks */
            public Op[] aOp; /* Space to hold the virtual machine's program */
            public Mem[] apArg; /* Arguments to currently executing user function */
            public VdbeCursor[] apCsr; /* One element of this array for each open cursor */
            public Mem[] aVar; /* Values for the OP_Variable opcode. */
            public string[] azVar; /* Name of variables */
            public int btreeMask; /* Bitmask of db.aDb[] entries referenced */
            public uint cacheCtr; /* VdbeCursor row cache generation counter */
            public bool changeCntOn; /* True to update the change-counter */
            public sqlite3 db; /* The database connection that owns this statement */
            public byte errorAction; /* Recovery action to do in case of an error */
            public bool expired; /* True if the VM needs to be recompiled */
            public int explain; /* True if EXPLAIN present on SQL command */
            public uint expmask; /* Binding to these vars invalidates VM */
            public int inVtabMethod; /* See comments above */
            public bool isPrepareV2; /* True if prepared with prepare_v2() */
            public int iStatement; /* Statement number (or 0 if has not opened stmt) */
            public uint magic; /* Magic number for sanity checking */
            public int minWriteFileFormat; /* Minimum file format for writable database files */
            public int nChange; /* Number of db changes made since last reset */
            public ushort nCursor; /* Number of slots in apCsr[] */
            public long nFkConstraint; /* Number of imm. FK constraints this VM */
            public int nFrame; /* Number of frames in pFrame list */
            public int nLabel; /* Number of labels used */
            public int nLabelAlloc; /* Number of slots allocated in aLabel[] */
            public int nMem; /* Number of memory locations currently allocated */
            public int nOp; /* Number of instructions in the program */
            public int nOpAlloc; /* Number of slots allocated for aOp[] */
            public ushort nResColumn; /* Number of columns in one row of the result set */
            public long nStmtDefCons; /* Number of def. constraints when stmt started */
            public short nVar; /* Number of entries in aVar[] */
            public byte okVar; /* True if azVar[] has been initialized */
            public int pc; /* The program counter */
            public VdbeFrame pFrame; /* Parent frame */
            public object pFree; /* Free this when deleting the vdbe */
            public Vdbe pNext; /* Linked list of VDBEs with the same Vdbe.db */
            public Vdbe pPrev; /* Linked list of VDBEs with the same Vdbe.db */
            public Mem[] pResultSet; /* Pointer to an array of results */
            public int rc; /* Value to return */
            public bool readOnly; /* True for read-only statements */
            public byte runOnlyOnce; /* Automatically expire on reset */
            public ulong startTime; /* Time when query started - used for profiling */
#if SQLITE_DEBUG
            public FILE trace; /* Write an execution trace here, if not NULL */
#endif
            public bool usesStmtJournal; /* True if uses a statement journal */
            public string zErrMsg; /* Error message written here */
            public string zSql = ""; /* Text of the SQL statement that generated this */

            public Vdbe Copy()
            {
                var cp = (Vdbe)MemberwiseClone();
                return cp;
            }

            public void CopyTo(Vdbe ct)
            {
                ct.db = db;
                ct.pPrev = pPrev;
                ct.pNext = pNext;
                ct.nOp = nOp;
                ct.nOpAlloc = nOpAlloc;
                ct.aOp = aOp;
                ct.nLabel = nLabel;
                ct.nLabelAlloc = nLabelAlloc;
                ct.aLabel = aLabel;
                ct.apArg = apArg;
                ct.aColName = aColName;
                ct.nCursor = nCursor;
                ct.apCsr = apCsr;
                ct.nVar = nVar;
                ct.aVar = aVar;
                ct.azVar = azVar;
                ct.okVar = okVar;
                ct.magic = magic;
                ct.nMem = nMem;
                ct.aMem = aMem;
                ct.cacheCtr = cacheCtr;
                ct.pc = pc;
                ct.rc = rc;
                ct.errorAction = errorAction;
                ct.nResColumn = nResColumn;
                ct.zErrMsg = zErrMsg;
                ct.pResultSet = pResultSet;
                ct.explain = explain;
                ct.changeCntOn = changeCntOn;
                ct.expired = expired;
                ct.minWriteFileFormat = minWriteFileFormat;
                ct.inVtabMethod = inVtabMethod;
                ct.usesStmtJournal = usesStmtJournal;
                ct.readOnly = readOnly;
                ct.nChange = nChange;
                ct.isPrepareV2 = isPrepareV2;
                ct.startTime = startTime;
                ct.btreeMask = btreeMask;
                ct.aMutex = aMutex;
                aCounter.CopyTo(ct.aCounter, 0);
                ct.zSql = zSql;
                ct.pFree = pFree;
#if SQLITE_DEBUG
                ct.trace = trace;
#endif
                ct.nFkConstraint = nFkConstraint;
                ct.nStmtDefCons = nStmtDefCons;
                ct.iStatement = iStatement;
                ct.pFrame = pFrame;
                ct.nFrame = nFrame;
                ct.expmask = expmask;

#if SQLITE_SSE
ct.fetchId = fetchId;
ct.lru = lru;
#endif
#if SQLITE_ENABLE_MEMORY_MANAGEMENT
ct.pLruPrev = pLruPrev;
ct.pLruNext = pLruNext;
#endif
            }
        }

        /*
        ** The following are allowed values for Vdbe.magic
        */
        //#define VDBE_MAGIC_INIT     0x26bceaa5    /* Building a VDBE program */
        //#define VDBE_MAGIC_RUN      0xbdf20da3    /* VDBE is ready to execute */
        //#define VDBE_MAGIC_HALT     0x519c2973    /* VDBE has completed execution */
        //#define VDBE_MAGIC_DEAD     0xb606c3c8    /* The VDBE has been deallocated */
        private const uint VDBE_MAGIC_INIT = 0x26bceaa5; /* Building a VDBE program */
        private const uint VDBE_MAGIC_RUN = 0xbdf20da3; /* VDBE is ready to execute */
        private const uint VDBE_MAGIC_HALT = 0x519c2973; /* VDBE has completed execution */

        private const uint VDBE_MAGIC_DEAD = 0xb606c3c8; /* The VDBE has been deallocated */
        /*
        ** Function prototypes
        */
        //void sqlite3VdbeFreeCursor(Vdbe *, VdbeCursor*);
        //void sqliteVdbePopStack(Vdbe*,int);
        //int sqlite3VdbeCursorMoveto(VdbeCursor*);
        //#if defined(SQLITE_DEBUG) || defined(VDBE_PROFILE)
        //void sqlite3VdbePrintOp(FILE*, int, Op*);
        //#endif
        //u32 sqlite3VdbeSerialTypeLen(u32);
        //u32 sqlite3VdbeSerialType(Mem*, int);
        //u32sqlite3VdbeSerialPut(unsigned char*, int, Mem*, int);
        //u32 sqlite3VdbeSerialGet(const unsigned char*, u32, Mem*);
        //void sqlite3VdbeDeleteAuxData(VdbeFunc*, int);

        //int sqlite2BtreeKeyCompare(BtCursor *, const void *, int, int, int *);
        //int sqlite3VdbeIdxKeyCompare(VdbeCursor*,UnpackedRecord*,int*);
        //int sqlite3VdbeIdxRowid(sqlite3 *, i64 *);
        //int sqlite3MemCompare(const Mem*, const Mem*, const CollSeq*);
        //int sqlite3VdbeExec(Vdbe*);
        //int sqlite3VdbeList(Vdbe*);
        //int sqlite3VdbeHalt(Vdbe*);
        //int sqlite3VdbeChangeEncoding(Mem *, int);
        //int sqlite3VdbeMemTooBig(Mem*);
        //int sqlite3VdbeMemCopy(Mem*, const Mem*);
        //void sqlite3VdbeMemShallowCopy(Mem*, const Mem*, int);
        //void sqlite3VdbeMemMove(Mem*, Mem*);
        //int sqlite3VdbeMemNulTerminate(Mem*);
        //int sqlite3VdbeMemSetStr(Mem*, const char*, int, u8, void(*)(void*));
        //void sqlite3VdbeMemSetInt64(Mem*, i64);
#if SQLITE_OMIT_FLOATING_POINT
    //# define sqlite3VdbeMemSetDouble sqlite3VdbeMemSetInt64
#else
        //void sqlite3VdbeMemSetDouble(Mem*, double);
#endif
        //void sqlite3VdbeMemSetNull(Mem*);
        //void sqlite3VdbeMemSetZeroBlob(Mem*,int);
        //void sqlite3VdbeMemSetRowSet(Mem*);
        //int sqlite3VdbeMemMakeWriteable(Mem*);
        //int sqlite3VdbeMemStringify(Mem*, int);
        //i64 sqlite3VdbeIntValue(Mem*);
        //int sqlite3VdbeMemIntegerify(Mem*);
        //double sqlite3VdbeRealValue(Mem*);
        //void sqlite3VdbeIntegerAffinity(Mem*);
        //int sqlite3VdbeMemRealify(Mem*);
        //int sqlite3VdbeMemNumerify(Mem*);
        //int sqlite3VdbeMemFromBtree(BtCursor*,int,int,int,Mem*);
        //void sqlite3VdbeMemRelease(Mem p);
        //void sqlite3VdbeMemReleaseExternal(Mem p);
        //int sqlite3VdbeMemFinalize(Mem*, FuncDef*);
        //const char *sqlite3OpcodeName(int);
        //int sqlite3VdbeMemGrow(Mem pMem, int n, int preserve);
        //int sqlite3VdbeCloseStatement(Vdbe *, int);
        //void sqlite3VdbeFrameDelete(VdbeFrame*);
        //int sqlite3VdbeFrameRestore(VdbeFrame *);
        //void sqlite3VdbeMemStoreType(Mem *pMem);  
#if !SQLITE_OMIT_FOREIGN_KEY
        //int sqlite3VdbeCheckFk(Vdbe *, int);
#else
    //# define sqlite3VdbeCheckFk(p,i) 0
    static int sqlite3VdbeCheckFk( Vdbe p, int i ) { return 0; }
#endif

#if !SQLITE_OMIT_SHARED_CACHE
//void sqlite3VdbeMutexArrayEnter(Vdbe *p);
#else
        //# define sqlite3VdbeMutexArrayEnter(p)
        private static void sqlite3VdbeMutexArrayEnter(Vdbe p)
        {
        }
#endif

        //int sqlite3VdbeMemTranslate(Mem*, u8);
        //#if SQLITE_DEBUG
        //  void sqlite3VdbePrintSql(Vdbe*);
        //  void sqlite3VdbeMemPrettyPrint(Mem pMem, char *zBuf);
        //#endif
        //int sqlite3VdbeMemHandleBom(Mem pMem);

#if !SQLITE_OMIT_INCRBLOB
//  int sqlite3VdbeMemExpandBlob(Mem *);
#else
        //  #define sqlite3VdbeMemExpandBlob(x) SQLITE_OK
        private static int sqlite3VdbeMemExpandBlob(Mem x)
        {
            return SQLITE_OK;
        }
#endif

        //#endif //* !_VDBEINT_H_) */
    }
}