using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace prjEvolutionAPI.Models;

public partial class EvolutionApiContext : DbContext
{
    public EvolutionApiContext()
    {
    }

    public EvolutionApiContext(DbContextOptions<EvolutionApiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TCompOrder> TCompOrders { get; set; }

    public virtual DbSet<TCompany> TCompanies { get; set; }

    public virtual DbSet<TCourse> TCourses { get; set; }

    public virtual DbSet<TCourseAccess> TCourseAccesses { get; set; }

    public virtual DbSet<TCourseAssignment> TCourseAssignments { get; set; }

    public virtual DbSet<TCourseChapter> TCourseChapters { get; set; }

    public virtual DbSet<TCourseHashTag> TCourseHashTags { get; set; }

    public virtual DbSet<TDepList> TDepLists { get; set; }

    public virtual DbSet<TEmpOrder> TEmpOrders { get; set; }

    public virtual DbSet<THashTagList> THashTagLists { get; set; }

    public virtual DbSet<TOption> TOptions { get; set; }

    public virtual DbSet<TPayment> TPayments { get; set; }

    public virtual DbSet<TPaymentDetail> TPaymentDetails { get; set; }

    public virtual DbSet<TQuestion> TQuestions { get; set; }

    public virtual DbSet<TQuiz> TQuizzes { get; set; }

    public virtual DbSet<TQuizAnswerDetail> TQuizAnswerDetails { get; set; }

    public virtual DbSet<TQuizResult> TQuizResults { get; set; }

    public virtual DbSet<TRefreshToken> TRefreshTokens { get; set; }

    public virtual DbSet<TUser> TUsers { get; set; }

    public virtual DbSet<TUserActionToken> TUserActionTokens { get; set; }

    public virtual DbSet<TVideo> TVideos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=EvolutionAPI;Integrated Security=True;Encrypt=False;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TCompOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__tCompOrd__C3905BAF16403164");

            entity.ToTable("tCompOrders");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.Amount).HasDefaultValue(0);
            entity.Property(e => e.BuyerCompanyId).HasColumnName("BuyerCompanyID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.IsPaid).HasDefaultValue(false);
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.BuyerCompany).WithMany(p => p.TCompOrders)
                .HasForeignKey(d => d.BuyerCompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tCompOrders_tCompanies");

            entity.HasOne(d => d.Course).WithMany(p => p.TCompOrders)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tCompOrders_tCourses");
        });

        modelBuilder.Entity<TCompany>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PK__tCompani__2D971C4C6B2E8A23");

            entity.ToTable("tCompanies");

            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CompanyEmail).HasMaxLength(100);
            entity.Property(e => e.CompanyName).HasMaxLength(100);
            entity.Property(e => e.CompanyPhone).HasMaxLength(20);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<TCourse>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__tCourses__C92D71874F46D64D");

            entity.ToTable("tCourses");

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CourseTitle).HasMaxLength(100);
            entity.Property(e => e.CoverImagePath).HasMaxLength(256);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsDraft).HasDefaultValue(true);

            entity.HasOne(d => d.Company).WithMany(p => p.TCourses)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tCourses_tCompanies");
        });

        modelBuilder.Entity<TCourseAccess>(entity =>
        {
            entity.HasKey(e => e.CourseAccessId).HasName("PK__tCourseA__14EFAA2929CECC6F");

            entity.ToTable("tCourseAccess");

            entity.Property(e => e.CourseAccessId).HasColumnName("CourseAccessID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Course).WithMany(p => p.TCourseAccesses)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_tCourseAccess_tCourses");

            entity.HasOne(d => d.User).WithMany(p => p.TCourseAccesses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_tCourseAccess_tUsers");
        });

        modelBuilder.Entity<TCourseAssignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PK__tCourseA__32499E572DC506E7");

            entity.ToTable("tCourseAssignment");

            entity.Property(e => e.AssignmentId).HasColumnName("AssignmentID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.IsPassed).HasDefaultValue(false);
            entity.Property(e => e.LastAttemptAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MinScore).HasDefaultValue(60);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Course).WithMany(p => p.TCourseAssignments)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tCourseAssignment_tCourses");

            entity.HasOne(d => d.User).WithMany(p => p.TCourseAssignments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tCourseAssignment_tUsers");
        });

        modelBuilder.Entity<TCourseChapter>(entity =>
        {
            entity.HasKey(e => e.ChapterId).HasName("PK__tCourseC__0893A34AEFF7A0DD");

            entity.ToTable("tCourseChapters");

            entity.Property(e => e.ChapterId).HasColumnName("ChapterID");
            entity.Property(e => e.ChapterTitle).HasMaxLength(100);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");

            entity.HasOne(d => d.Course).WithMany(p => p.TCourseChapters)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tCourseChapters_tCourses");
        });

        modelBuilder.Entity<TCourseHashTag>(entity =>
        {
            entity.HasKey(e => e.CourseHashTagId).HasName("PK__tCourseH__53192E56809A803D");

            entity.ToTable("tCourseHashTag");

            entity.Property(e => e.CourseHashTagId).HasColumnName("CourseHashTagID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.TagId).HasColumnName("TagID");

            entity.HasOne(d => d.Course).WithMany(p => p.TCourseHashTags)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tCourseHashTag_tCourses");

            entity.HasOne(d => d.Tag).WithMany(p => p.TCourseHashTags)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tCourseHashTag_tHashTagList");
        });

        modelBuilder.Entity<TDepList>(entity =>
        {
            entity.HasKey(e => e.DepId).HasName("PK__tDepList__DB9CAA7F3ABCD411");

            entity.ToTable("tDepList");

            entity.Property(e => e.DepId).HasColumnName("DepID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.DepName).HasMaxLength(50);

            entity.HasOne(d => d.Company).WithMany(p => p.TDepLists)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tDepList_tCompanies");
        });

        modelBuilder.Entity<TEmpOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__tEmpOrde__C3905BAF7B7F6CC2");

            entity.ToTable("tEmpOrders");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.Amount).HasDefaultValue(0);
            entity.Property(e => e.BuyerUserId).HasColumnName("BuyerUserID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.IsPaid).HasDefaultValue(false);
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.BuyerUser).WithMany(p => p.TEmpOrders)
                .HasForeignKey(d => d.BuyerUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tEmpOrders_tUsers");

            entity.HasOne(d => d.Course).WithMany(p => p.TEmpOrders)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tEmpOrders_tCourses");
        });

        modelBuilder.Entity<THashTagList>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("PK__tHashTag__657CFA4CF06E5E6B");

            entity.ToTable("tHashTagList");

            entity.Property(e => e.TagId).HasColumnName("TagID");
            entity.Property(e => e.TagName).HasMaxLength(50);
        });

        modelBuilder.Entity<TOption>(entity =>
        {
            entity.HasKey(e => e.OptionId).HasName("PK__tOptions__92C7A1DF277BFC3C");

            entity.ToTable("tOptions");

            entity.Property(e => e.OptionId).HasColumnName("OptionID");
            entity.Property(e => e.OptionText).HasMaxLength(200);
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

            entity.HasOne(d => d.Question).WithMany(p => p.TOptions)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tOptions_tQuestions");
        });

        modelBuilder.Entity<TPayment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__tPayment__9B556A58BDEACBE0");

            entity.ToTable("tPayments");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaidAt).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(20);
        });

        modelBuilder.Entity<TPaymentDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__tPayment__135C314D9E9C356B");

            entity.ToTable("tPaymentDetails");

            entity.Property(e => e.DetailId).HasColumnName("DetailID");
            entity.Property(e => e.CompOrderId).HasColumnName("CompOrderID");
            entity.Property(e => e.EmpOrderId).HasColumnName("EmpOrderID");
            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");

            entity.HasOne(d => d.CompOrder).WithMany(p => p.TPaymentDetails)
                .HasForeignKey(d => d.CompOrderId)
                .HasConstraintName("FK__tPaymentD__CompO__00DF2177");

            entity.HasOne(d => d.EmpOrder).WithMany(p => p.TPaymentDetails)
                .HasForeignKey(d => d.EmpOrderId)
                .HasConstraintName("FK__tPaymentD__EmpOr__01D345B0");

            entity.HasOne(d => d.Payment).WithMany(p => p.TPaymentDetails)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tPaymentD__Payme__7FEAFD3E");
        });

        modelBuilder.Entity<TQuestion>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PK__tQuestio__0DC06F8CB0FB2F76");

            entity.ToTable("tQuestions");

            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            entity.Property(e => e.QuizId).HasColumnName("QuizID");

            entity.HasOne(d => d.Quiz).WithMany(p => p.TQuestions)
                .HasForeignKey(d => d.QuizId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tQuestions_tQuizzes");
        });

        modelBuilder.Entity<TQuiz>(entity =>
        {
            entity.HasKey(e => e.QuizId).HasName("PK__tQuizzes__8B42AE6E5B9624F8");

            entity.ToTable("tQuizzes");

            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.Title).HasMaxLength(256);

            entity.HasOne(d => d.Course).WithMany(p => p.TQuizzes)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tQuizzes_tCourses");
        });

        modelBuilder.Entity<TQuizAnswerDetail>(entity =>
        {
            entity.HasKey(e => e.AnswerDetailId).HasName("PK__tQuizAns__F6D258130DE57B86");

            entity.ToTable("tQuizAnswerDetails");

            entity.Property(e => e.AnswerDetailId).HasColumnName("AnswerDetailID");
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            entity.Property(e => e.ResultId).HasColumnName("ResultID");
            entity.Property(e => e.SelectedOptionId).HasColumnName("SelectedOptionID");

            entity.HasOne(d => d.Question).WithMany(p => p.TQuizAnswerDetails)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tQuizAnswerDetails_tQuestions");

            entity.HasOne(d => d.Result).WithMany(p => p.TQuizAnswerDetails)
                .HasForeignKey(d => d.ResultId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tQuizAnswerDetails_tQuizResults");

            entity.HasOne(d => d.SelectedOption).WithMany(p => p.TQuizAnswerDetails)
                .HasForeignKey(d => d.SelectedOptionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tQuizAnswerDetails_tOptions");
        });

        modelBuilder.Entity<TQuizResult>(entity =>
        {
            entity.HasKey(e => e.ResultId).HasName("PK__tQuizRes__97690228E788BDDA");

            entity.ToTable("tQuizResults");

            entity.Property(e => e.ResultId).HasColumnName("ResultID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Quiz).WithMany(p => p.TQuizResults)
                .HasForeignKey(d => d.QuizId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tQuizResults_tQuizzes");

            entity.HasOne(d => d.User).WithMany(p => p.TQuizResults)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tQuizResults_tUsers");
        });

        modelBuilder.Entity<TRefreshToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__tRefresh__658FEEEAA6FEE6F7");

            entity.ToTable("tRefreshTokens");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.Token).HasMaxLength(512);
            entity.Property(e => e.UserAgent)
                .HasMaxLength(512)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.TRefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefreshTokens_Users");
        });

        modelBuilder.Entity<TUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__tUsers__1788CCAC228FC348");

            entity.ToTable("tUsers");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.LockoutEndTime).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(512);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.UserPic).HasMaxLength(256);
            entity.Property(e => e.UserStatus).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Company).WithMany(p => p.TUsers)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tUsers_tCompanies");

            entity.HasOne(d => d.UserDepNavigation).WithMany(p => p.TUsers)
                .HasForeignKey(d => d.UserDep)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tUsers_tDepList");
        });

        modelBuilder.Entity<TUserActionToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__tUserAct__658FEEEAFD9EFC6D");

            entity.ToTable("tUserActionTokens");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.Token).HasMaxLength(100);
            entity.Property(e => e.TokenType).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.TUserActionTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserActionTokens_User");
        });

        modelBuilder.Entity<TVideo>(entity =>
        {
            entity.HasKey(e => e.VideoId).HasName("PK__tVideos__BAE5124A4A52CA88");

            entity.ToTable("tVideos");

            entity.Property(e => e.VideoId).HasColumnName("VideoID");
            entity.Property(e => e.ChapterId).HasColumnName("ChapterID");
            entity.Property(e => e.VideoTitle).HasMaxLength(100);
            entity.Property(e => e.VideoUrl).HasMaxLength(256);

            entity.HasOne(d => d.Chapter).WithMany(p => p.TVideos)
                .HasForeignKey(d => d.ChapterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tVideos_tCourseChapters");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
