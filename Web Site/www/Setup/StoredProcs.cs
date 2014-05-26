using System.Text;

namespace SnitzUI.Setup
{
    public partial class Process
    {
        private StringBuilder strSQL = new StringBuilder();

        private string UpdateCounts()
        {
            strSQL.AppendLine("CREATE PROCEDURE [dbo].[snitz_updateCounts]");
            strSQL.AppendLine("AS");
            strSQL.AppendLine("BEGIN");
            strSQL.AppendLine("	-- SET NOCOUNT ON added to prevent extra result sets from");
            strSQL.AppendLine("	-- interfering with SELECT statements.");
            strSQL.AppendLine("	SET NOCOUNT ON;");
            
            strSQL.AppendLine("/* Update Forum Topic Counts */");
            
            strSQL.AppendLine("CREATE TABLE #F_T_COUNT (");
            strSQL.AppendLine("    FORUM_ID int,");
            strSQL.AppendLine("    T_COUNT int");
            strSQL.AppendLine(");");
            
            strSQL.AppendLine("UPDATE FORUM_FORUM SET F_TOPICS = 0;");
            
            strSQL.AppendLine("INSERT INTO #F_T_COUNT");
            strSQL.AppendLine("  SELECT FORUM_ID, COUNT(FORUM_ID) FROM FORUM_TOPICS WHERE T_STATUS<=1 GROUP By FORUM_ID ;");
            
            strSQL.AppendLine("UPDATE FORUM_FORUM SET F_TOPICS = T_COUNT FROM FORUM_FORUM F INNER JOIN #F_T_COUNT T ON T.FORUM_ID=F.FORUM_ID;");
            strSQL.AppendLine("  ");
            
            strSQL.AppendLine("/* Update Forum Archived Topics Count  */");
            
            strSQL.AppendLine("DELETE FROM #F_T_COUNT;");
            
            strSQL.AppendLine("UPDATE FORUM_FORUM SET F_A_TOPICS = 0;");
            
            strSQL.AppendLine("INSERT INTO #F_T_COUNT");
            strSQL.AppendLine("  SELECT FORUM_ID, COUNT(FORUM_ID) FROM FORUM_A_TOPICS WHERE T_STATUS<=1 GROUP By FORUM_ID ;");
            
            strSQL.AppendLine("UPDATE FORUM_FORUM SET F_A_TOPICS = T_COUNT FROM FORUM_FORUM F INNER JOIN #F_T_COUNT T ON T.FORUM_ID=F.FORUM_ID;");
            
            strSQL.AppendLine("DROP TABLE #F_T_COUNT;");
            
            
            strSQL.AppendLine("/* Update Topic Replies Counts now */");
            
            strSQL.AppendLine("CREATE TABLE #T_R_COUNT (");
            strSQL.AppendLine("   TOPIC_ID int,");
            strSQL.AppendLine("   R_COUNT int");
            strSQL.AppendLine(");");
            
            strSQL.AppendLine("INSERT INTO #T_R_COUNT ");
            strSQL.AppendLine("  SELECT TOPIC_ID, COUNT(REPLY_ID) FROM FORUM_REPLY WHERE R_STATUS<=1 GROUP BY TOPIC_ID ;");
            
            strSQL.AppendLine("UPDATE FORUM_TOPICS SET T_REPLIES = 0 WHERE T_STATUS<=1;");
            
            strSQL.AppendLine("UPDATE FORUM_TOPICS SET T_REPLIES = R_COUNT  FROM FORUM_TOPICS T INNER JOIN #T_R_COUNT TR ON T.TOPIC_ID = TR.TOPIC_ID");
            strSQL.AppendLine("   WHERE T.T_STATUS<=1;");
            
            strSQL.AppendLine("/* Update Archived Topics Replies Count Now */");
            
            strSQL.AppendLine("DELETE FROM #T_R_COUNT;");
            
            strSQL.AppendLine("INSERT INTO #T_R_COUNT ");
            strSQL.AppendLine("  SELECT TOPIC_ID, COUNT(REPLY_ID) FROM FORUM_A_REPLY GROUP BY TOPIC_ID;");
            
            strSQL.AppendLine("UPDATE FORUM_A_TOPICS SET T_REPLIES = 0;");
            
            strSQL.AppendLine("UPDATE FORUM_A_TOPICS SET T_REPLIES = R_COUNT FROM FORUM_A_TOPICS T INNER JOIN #T_R_COUNT TR ON T.TOPIC_ID = TR.TOPIC_ID;");
            
            strSQL.AppendLine("DROP TABLE #T_R_COUNT;");
            
            strSQL.AppendLine("/* Update Last post Date */ ");
            
            strSQL.AppendLine("CREATE TABLE #T_POST_DATA (");
            strSQL.AppendLine("   TOPIC_ID int,");
            strSQL.AppendLine("   LAST_POST nvarchar(14)");
            strSQL.AppendLine(");");
            
            strSQL.AppendLine("INSERT INTO #T_POST_DATA");
            strSQL.AppendLine("  SELECT TOPIC_ID, MAX(R_DATE) FROM FORUM_REPLY WHERE R_STATUS<=1 GROUP BY TOPIC_ID;");
            
            strSQL.AppendLine("UPDATE FORUM_TOPICS SET T_LAST_POST = LAST_POST FROM FORUM_TOPICS INNER JOIN #T_POST_DATA PT  ON FORUM_TOPICS.TOPIC_ID = PT.TOPIC_ID;");
            
            strSQL.AppendLine("DELETE FROM #T_POST_DATA;");
            
            strSQL.AppendLine("UPDATE FORUM_TOPICS SET T_LAST_POST=T_DATE, T_LAST_POST_AUTHOR=T_AUTHOR WHERE T_REPLIES=0;");
            
            strSQL.AppendLine("/* Update Last post Date */ ");
            
            strSQL.AppendLine("INSERT INTO #T_POST_DATA");
            strSQL.AppendLine("  SELECT TOPIC_ID, MAX(R_DATE) FROM FORUM_A_REPLY GROUP BY TOPIC_ID;");
            
            strSQL.AppendLine("UPDATE FORUM_A_TOPICS SET T_LAST_POST = LAST_POST FROM FORUM_A_TOPICS T INNER JOIN #T_POST_DATA TP  ON T.TOPIC_ID = TP.TOPIC_ID;");
            
            strSQL.AppendLine("UPDATE FORUM_A_TOPICS SET T_LAST_POST=T_DATE, T_LAST_POST_AUTHOR=T_AUTHOR WHERE T_REPLIES=0;");
            
            strSQL.AppendLine("DROP TABLE #T_POST_DATA;");
            strSQL.AppendLine("/* Now find the reply ID for the posts that have more than 0 replies */");
            
            strSQL.AppendLine("CREATE TABLE #T_L_REPLY_ID (");
            strSQL.AppendLine("   TOPIC_ID int,");
            strSQL.AppendLine("   REPLY_ID int");
            strSQL.AppendLine(");");
            
            strSQL.AppendLine("CREATE TABLE #T_L_A_REPLY_ID (");
            strSQL.AppendLine("   TOPIC_ID int,");
            strSQL.AppendLine("   REPLY_ID int");
            strSQL.AppendLine(");");
            
            strSQL.AppendLine("INSERT INTO #T_L_REPLY_ID");
            strSQL.AppendLine("  SELECT T.TOPIC_ID, MAX(REPLY_ID) FROM FORUM_REPLY R INNER JOIN FORUM_TOPICS T ON R.TOPIC_ID=T.TOPIC_ID");
            strSQL.AppendLine("       WHERE T.T_LAST_POST=R_DATE AND T_STATUS<=1 GROUP BY T.TOPIC_ID;");
            
            strSQL.AppendLine("UPDATE FORUM_TOPICS SET T_LAST_POST_REPLY_ID = REPLY_ID FROM FORUM_TOPICS T LEFT JOIN #T_L_REPLY_ID TL ON TL.TOPIC_ID = T.TOPIC_ID;");
            
            strSQL.AppendLine("DELETE FROM #T_L_REPLY_ID;");
            
            strSQL.AppendLine("/* Now find the reply ID for the posts that have more than 0 replies in archived topics */");
            
            strSQL.AppendLine("INSERT INTO #T_L_A_REPLY_ID");
            strSQL.AppendLine("  SELECT T.TOPIC_ID, MAX(REPLY_ID) FROM FORUM_A_REPLY R INNER JOIN FORUM_A_TOPICS T ON R.TOPIC_ID=T.TOPIC_ID");
            strSQL.AppendLine("       WHERE T.T_LAST_POST=R_DATE GROUP BY T.TOPIC_ID;");
            
            strSQL.AppendLine("--UPDATE FORUM_A_TOPICS SET T_LAST_POST_REPLY_ID = REPLY_ID FROM FORUM_A_TOPICS T INNER JOIN #T_L_REPLY_ID TL ON TL.TOPIC_ID = T.TOPIC_ID;");
            
            strSQL.AppendLine("/* Now found the author ID for the last reply */");
            
            strSQL.AppendLine("CREATE TABLE #T_L_REPLY_AUTHOR(");
            strSQL.AppendLine("   TOPIC_ID int,");
            strSQL.AppendLine("   AUTHOR int");
            strSQL.AppendLine(");");
            
            strSQL.AppendLine("INSERT INTO #T_L_REPLY_AUTHOR");
            strSQL.AppendLine("  SELECT T.TOPIC_ID, R.R_AUTHOR FROM FORUM_TOPICS T INNER JOIN FORUM_REPLY R ON T.TOPIC_ID=R.TOPIC_ID");
            strSQL.AppendLine("       INNER JOIN #T_L_REPLY_ID ON T.TOPIC_ID= #T_L_REPLY_ID.TOPIC_ID ");
            strSQL.AppendLine("    WHERE #T_L_REPLY_ID.REPLY_ID = R.REPLY_ID AND T_STATUS<=1;");
            
            strSQL.AppendLine("UPDATE FORUM_TOPICS SET T_LAST_POST_AUTHOR = AUTHOR FROM FORUM_TOPICS T INNER JOIN #T_L_REPLY_AUTHOR TL ON TL.TOPIC_ID = T.TOPIC_ID;");
            
            strSQL.AppendLine("DELETE FROM #T_L_REPLY_AUTHOR;");
            
            strSQL.AppendLine("INSERT INTO #T_L_REPLY_AUTHOR");
            strSQL.AppendLine("  SELECT T.TOPIC_ID, R.R_AUTHOR FROM FORUM_A_TOPICS T INNER JOIN FORUM_A_REPLY R ON T.TOPIC_ID=R.TOPIC_ID");
            strSQL.AppendLine("     INNER JOIN #T_L_A_REPLY_ID ON T.TOPIC_ID = #T_L_A_REPLY_ID.TOPIC_ID");
            strSQL.AppendLine("    WHERE #T_L_A_REPLY_ID.REPLY_ID = R.REPLY_ID;");
            
            strSQL.AppendLine("UPDATE FORUM_A_TOPICS SET T_LAST_POST_AUTHOR = AUTHOR FROM FORUM_A_TOPICS T INNER JOIN #T_L_REPLY_AUTHOR TL ON TL.TOPIC_ID = T.TOPIC_ID;");
            
            strSQL.AppendLine("DROP TABLE #T_L_REPLY_AUTHOR;");
            
            strSQL.AppendLine("DROP TABLE #T_L_REPLY_ID;");
            
            strSQL.AppendLine("DROP TABLE #T_L_A_REPLY_ID;");
            
            
            strSQL.AppendLine("/* Now to current step 3, unmoderated replies per topic - removed by RR since it doesn't make sense for 3.3.x*/");
            
            strSQL.AppendLine("/* Update Topic Replies Counts now */");
            
            strSQL.AppendLine("/*CREATE TABLE #T_R_COUNT1 (");
            strSQL.AppendLine("   TOPIC_ID int,");
            strSQL.AppendLine("   R_COUNT int");
            strSQL.AppendLine(");");
            
            strSQL.AppendLine("INSERT INTO #T_R_COUNT1 ");
            strSQL.AppendLine("  SELECT TOPIC_ID, COUNT(REPLY_ID) FROM FORUM_REPLY WHERE R_STATUS=2 OR R_STATUS=3 GROUP BY TOPIC_ID ;");
            
            strSQL.AppendLine("UPDATE FORUM_TOPICS SET T_REPLIES = 0 WHERE T_STATUS<=1;");
            
            strSQL.AppendLine("UPDATE FORUM_TOPICS SET T_REPLIES = R_COUNT FROM FORUM_TOPICS T INNER JOIN #T_R_COUNT1 TR ON T.TOPIC_ID = TR.TOPIC_ID");
            strSQL.AppendLine("   WHERE T.T_STATUS<=1;");
            
            strSQL.AppendLine("DROP TABLE #T_R_COUNT1;*/");
            
            strSQL.AppendLine("/* Now to step 4 */");
            
            strSQL.AppendLine("/* Count replies per forum */");
            
            strSQL.AppendLine("CREATE TABLE #F_R_COUNT (");
            strSQL.AppendLine("    FORUM_ID int,");
            strSQL.AppendLine("    R_COUNT int");
            strSQL.AppendLine(");");
            
            strSQL.AppendLine("INSERT INTO #F_R_COUNT ");
            strSQL.AppendLine("  SELECT R.FORUM_ID, COUNT(REPLY_ID) FROM FORUM_TOPICS T INNER JOIN FORUM_REPLY R On T.TOPIC_ID=R.TOPIC_ID");
            strSQL.AppendLine("     WHERE T.T_STATUS<=1 AND R_STATUS<=1 GROUP By R.FORUM_ID;");
            
            strSQL.AppendLine("UPDATE FORUM_FORUM SET F_COUNT=F_TOPICS WHERE F_TYPE<>1;");
            
            strSQL.AppendLine("UPDATE FORUM_FORUM SET F_COUNT = F_COUNT + R_COUNT FROM FORUM_FORUM F INNER JOIN #F_R_COUNT FR ON F.FORUM_ID = FR.FORUM_ID;");
            
            strSQL.AppendLine("DELETE FROM #F_R_COUNT;");
            
            strSQL.AppendLine("INSERT INTO #F_R_COUNT ");
            strSQL.AppendLine("  SELECT R.FORUM_ID, COUNT(REPLY_ID) FROM FORUM_A_TOPICS T INNER JOIN FORUM_A_REPLY R On T.TOPIC_ID=R.TOPIC_ID GROUP By R.FORUM_ID;");
            
            strSQL.AppendLine("UPDATE FORUM_FORUM SET F_A_COUNT=F_A_TOPICS WHERE F_TYPE<>1;");
            
            strSQL.AppendLine("UPDATE FORUM_FORUM SET F_A_COUNT = F_A_COUNT + R_COUNT FROM FORUM_FORUM F INNER JOIN #F_R_COUNT FR ON F.FORUM_ID = FR.FORUM_ID");
            strSQL.AppendLine(" ");
            strSQL.AppendLine("DROP TABLE  #F_R_COUNT;");
            
            strSQL.AppendLine("/* Update Last Post Per Forum */");
            
            strSQL.AppendLine("CREATE TABLE #F_POST_DATA (");
            strSQL.AppendLine("   FORUM_ID int,");
            strSQL.AppendLine("   LAST_POST varchar(50)");
            strSQL.AppendLine(");");
            
            
            strSQL.AppendLine("INSERT INTO #F_POST_DATA");
            strSQL.AppendLine("  SELECT FORUM_ID, MAX(T_LAST_POST) FROM FORUM_TOPICS WHERE T_STATUS<=1 GROUP BY FORUM_ID;");
            
            strSQL.AppendLine("UPDATE FORUM_FORUM SET F_LAST_POST = LAST_POST FROM FORUM_FORUM F LEFT JOIN #F_POST_DATA FP  ON F.FORUM_ID = FP.FORUM_ID;");
            
            strSQL.AppendLine("DROP TABLE #F_POST_DATA;");
            strSQL.AppendLine("/* Update Last Post TOPIC_ID */");
            strSQL.AppendLine("CREATE TABLE #F_TOPIC_ID (");
            strSQL.AppendLine("   FORUM_ID int,");
            strSQL.AppendLine("   TOPIC_ID int");
            strSQL.AppendLine(");");
            strSQL.AppendLine("INSERT INTO #F_TOPIC_ID");
            strSQL.AppendLine("  SELECT F.FORUM_ID, MAX(T.TOPIC_ID) FROM FORUM_FORUM F INNER JOIN FORUM_TOPICS T On F.FORUM_ID=T.FORUM_ID");
            strSQL.AppendLine("   WHERE F.F_LAST_POST = T.T_LAST_POST and T.T_STATUS<=1 GROUP BY F.FORUM_ID;");
            
            strSQL.AppendLine("UPDATE FORUM_FORUM SET F_LAST_POST_TOPIC_ID = TOPIC_ID FROM FORUM_FORUM F LEFT JOIN #F_TOPIC_ID FT ON F.FORUM_ID = FT.FORUM_ID;");
            strSQL.AppendLine("   ");
            strSQL.AppendLine("/* Now Update for Author ID */");
            strSQL.AppendLine("UPDATE FORUM_FORUM SET F_LAST_POST_AUTHOR=NULL");
            strSQL.AppendLine("UPDATE FORUM_FORUM SET F_LAST_POST_AUTHOR=T_LAST_POST_AUTHOR,F_LAST_POST_REPLY_ID=T_LAST_POST_REPLY_ID ");
            strSQL.AppendLine("FROM ((FORUM_FORUM F INNER JOIN #F_TOPIC_ID ON F.FORUM_ID=#F_TOPIC_ID.FORUM_ID) LEFT JOIN FORUM_TOPICS T On #F_TOPIC_ID.TOPIC_ID=T.TOPIC_ID);");
            
            strSQL.AppendLine("DROP TABLE #F_TOPIC_ID;");
            
            strSQL.AppendLine("CREATE  TABLE #T_TOPICS (");
            strSQL.AppendLine("   COUNT_ID int,");
            strSQL.AppendLine("   TOPICS int,");
            strSQL.AppendLine("   A_TOPICS int,");
            strSQL.AppendLine("   POSTS int,");
            strSQL.AppendLine("   A_POSTS int");
            strSQL.AppendLine(");");
            
            strSQL.AppendLine("INSERT INTO #T_TOPICS");
            strSQL.AppendLine("  SELECT 1, SUM(F_TOPICS), SUM(F_A_TOPICS),SUM(F_COUNT),SUM(F_A_COUNT) FROM FORUM_FORUM WHERE F_TYPE<>1;");
            
            strSQL.AppendLine("UPDATE FORUM_TOTALS SET T_COUNT = TOPICS, T_A_COUNT = A_TOPICS, P_COUNT=POSTS, P_A_COUNT=A_POSTS");
            strSQL.AppendLine("  FROM FORUM_TOTALS FT INNER JOIN #T_TOPICS TT ON FT.COUNT_ID=TT.COUNT_ID;");
            
            strSQL.AppendLine("DROP TABLE #T_TOPICS;");
            
            strSQL.AppendLine("CREATE TABLE #T_MEMBERS (");
            strSQL.AppendLine("   COUNT_ID int,");
            strSQL.AppendLine("   MEMBERS int");
            strSQL.AppendLine(");");
            
            strSQL.AppendLine("INSERT INTO #T_MEMBERS");
            strSQL.AppendLine("  SELECT 1, COUNT(MEMBER_ID) FROM FORUM_MEMBERS;");
            
            strSQL.AppendLine("UPDATE FORUM_TOTALS SET U_COUNT = MEMBERS FROM FORUM_TOTALS FT INNER JOIN #T_MEMBERS TM ON FT.COUNT_ID=TM.COUNT_ID;");
            
            strSQL.AppendLine("DROP TABLE #T_MEMBERS;");
            
            strSQL.AppendLine("End");
            
            strSQL.AppendLine("GO");
            

            return strSQL.ToString();
        }
        
    }
}