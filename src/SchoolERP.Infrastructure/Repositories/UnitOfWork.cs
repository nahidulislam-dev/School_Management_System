using Microsoft.EntityFrameworkCore.Storage;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.AcademicYear.Interfaces;
using SchoolERP.Application.Features.ClassSubject.Interfaces;
using SchoolERP.Application.Features.Designation.Interfaces;
using SchoolERP.Application.Features.Employee.Interfaces;
using SchoolERP.Application.Features.EmployeeAttendance.Interfaces;
using SchoolERP.Application.Features.EmployeeSalary.Interfaces;
using SchoolERP.Application.Features.Exam.Interfaces;
using SchoolERP.Application.Features.ExamResult.Interfaces;
using SchoolERP.Application.Features.ExamSchedule.Interfaces;
using SchoolERP.Application.Features.ExamType.Interfaces;
using SchoolERP.Application.Features.ExamWeightItem.Interfaces;
using SchoolERP.Application.Features.ExamWeightSetup.Interfaces;
using SchoolERP.Application.Features.FeeCollection.Interfaces;
using SchoolERP.Application.Features.FeeStructure.Interfaces;
using SchoolERP.Application.Features.FeeType.Interfaces;
using SchoolERP.Application.Features.FinalResult.Interfaces;
using SchoolERP.Application.Features.Guardian.Interfaces;
using SchoolERP.Application.Features.Notice.Interfaces;
using SchoolERP.Application.Features.PasswordResetToken.Interfaces;
using SchoolERP.Application.Features.Permission.Interfaces;
using SchoolERP.Application.Features.RefreshToken.Interfaces;
using SchoolERP.Application.Features.Result.Interfaces;
using SchoolERP.Application.Features.Role.Interfaces;
using SchoolERP.Application.Features.RolePermission.Interfaces;
using SchoolERP.Application.Features.School.Interfaces;
using SchoolERP.Application.Features.SchoolClass.Interfaces;
using SchoolERP.Application.Features.Section.Interfaces;
using SchoolERP.Application.Features.SmsLog.Interfaces;
using SchoolERP.Application.Features.SmsTemplate.Interfaces;
using SchoolERP.Application.Features.Student.Interfaces;
using SchoolERP.Application.Features.StudentAttendance.Interfaces;
using SchoolERP.Application.Features.StudentGuardian.Interfaces;
using SchoolERP.Application.Features.Subject.Interfaces;
using SchoolERP.Application.Features.SubjectTeacher.Interfaces;
using SchoolERP.Application.Features.Teacher.Interfaces;
using SchoolERP.Application.Features.User.Interfaces;
using SchoolERP.Application.Features.UserRole.Interfaces;
using SchoolERP.Infrastructure.Persistence.Context;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core Unit of Work. Lazily instantiates each feature repository against a
/// single shared <see cref="SchoolERPDbContext"/> so all changes within a request
/// are committed together in one transaction via <see cref="SaveChangesAsync"/>.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly SchoolERPDbContext _context;
    private bool _disposed;

    private IAcademicYearRepository? _academicYearRepository;
    private IClassSubjectRepository? _classSubjectRepository;
    private IDesignationRepository? _designationRepository;
    private IEmployeeRepository? _employeeRepository;
    private IEmployeeAttendanceRepository? _employeeAttendanceRepository;
    private IEmployeeSalaryRepository? _employeeSalaryRepository;
    private IExamRepository? _examRepository;
    private IExamScheduleRepository? _examScheduleRepository;
    private IExamTypeRepository? _examTypeRepository;
    private IFeeCollectionRepository? _feeCollectionRepository;
    private IFeeStructureRepository? _feeStructureRepository;
    private IFeeTypeRepository? _feeTypeRepository;
    private IGuardianRepository? _guardianRepository;
    private INoticeRepository? _noticeRepository;
    private IPermissionRepository? _permissionRepository;
    private IRefreshTokenRepository? _refreshTokenRepository;
    private IPasswordResetTokenRepository? _passwordResetTokenRepository;
    private IResultRepository? _resultRepository;
    private IExamResultRepository? _examResultRepository;
    private IExamWeightSetupRepository? _examWeightSetupRepository;
    private IExamWeightItemRepository? _examWeightItemRepository;
    private IFinalResultRepository? _finalResultRepository;
    private IRoleRepository? _roleRepository;
    private IRolePermissionRepository? _rolePermissionRepository;
    private ISchoolRepository? _schoolRepository;
    private ISchoolClassRepository? _schoolClassRepository;
    private ISectionRepository? _sectionRepository;
    private ISmsLogRepository? _smsLogRepository;
    private ISmsTemplateRepository? _smsTemplateRepository;
    private IStudentRepository? _studentRepository;
    private IStudentAttendanceRepository? _studentAttendanceRepository;
    private IStudentGuardianRepository? _studentGuardianRepository;
    private ISubjectRepository? _subjectRepository;
    private ISubjectTeacherRepository? _subjectTeacherRepository;
    private ITeacherRepository? _teacherRepository;
    private IUserRepository? _userRepository;
    private IUserRoleRepository? _userRoleRepository;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(SchoolERPDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public IAcademicYearRepository AcademicYearRepository => _academicYearRepository ??= new AcademicYearRepository(_context);

    /// <inheritdoc />
    public IClassSubjectRepository ClassSubjectRepository => _classSubjectRepository ??= new ClassSubjectRepository(_context);

    /// <inheritdoc />
    public IDesignationRepository DesignationRepository => _designationRepository ??= new DesignationRepository(_context);

    /// <inheritdoc />
    public IEmployeeRepository EmployeeRepository => _employeeRepository ??= new EmployeeRepository(_context);

    /// <inheritdoc />
    public IEmployeeAttendanceRepository EmployeeAttendanceRepository => _employeeAttendanceRepository ??= new EmployeeAttendanceRepository(_context);

    /// <inheritdoc />
    public IEmployeeSalaryRepository EmployeeSalaryRepository => _employeeSalaryRepository ??= new EmployeeSalaryRepository(_context);

    /// <inheritdoc />
    public IExamRepository ExamRepository => _examRepository ??= new ExamRepository(_context);

    /// <inheritdoc />
    public IExamScheduleRepository ExamScheduleRepository => _examScheduleRepository ??= new ExamScheduleRepository(_context);

    /// <inheritdoc />
    public IExamTypeRepository ExamTypeRepository => _examTypeRepository ??= new ExamTypeRepository(_context);

    /// <inheritdoc />
    public IFeeCollectionRepository FeeCollectionRepository => _feeCollectionRepository ??= new FeeCollectionRepository(_context);

    /// <inheritdoc />
    public IFeeStructureRepository FeeStructureRepository => _feeStructureRepository ??= new FeeStructureRepository(_context);

    /// <inheritdoc />
    public IFeeTypeRepository FeeTypeRepository => _feeTypeRepository ??= new FeeTypeRepository(_context);

    /// <inheritdoc />
    public IGuardianRepository GuardianRepository => _guardianRepository ??= new GuardianRepository(_context);

    /// <inheritdoc />
    public INoticeRepository NoticeRepository => _noticeRepository ??= new NoticeRepository(_context);

    /// <inheritdoc />
    public IPermissionRepository PermissionRepository => _permissionRepository ??= new PermissionRepository(_context);

    /// <inheritdoc />
    public IRefreshTokenRepository RefreshTokenRepository => _refreshTokenRepository ??= new RefreshTokenRepository(_context);

    /// <inheritdoc />
    public IPasswordResetTokenRepository PasswordResetTokenRepository => _passwordResetTokenRepository ??= new PasswordResetTokenRepository(_context);

    /// <inheritdoc />
    public IResultRepository ResultRepository => _resultRepository ??= new ResultRepository(_context);

    /// <inheritdoc />
    public IExamResultRepository ExamResultRepository => _examResultRepository ??= new ExamResultRepository(_context);

    /// <inheritdoc />
    public IExamWeightSetupRepository ExamWeightSetupRepository => _examWeightSetupRepository ??= new ExamWeightSetupRepository(_context);

    /// <inheritdoc />
    public IExamWeightItemRepository ExamWeightItemRepository => _examWeightItemRepository ??= new ExamWeightItemRepository(_context);

    /// <inheritdoc />
    public IFinalResultRepository FinalResultRepository => _finalResultRepository ??= new FinalResultRepository(_context);

    /// <inheritdoc />
    public IRoleRepository RoleRepository => _roleRepository ??= new RoleRepository(_context);

    /// <inheritdoc />
    public IRolePermissionRepository RolePermissionRepository => _rolePermissionRepository ??= new RolePermissionRepository(_context);

    /// <inheritdoc />
    public ISchoolRepository SchoolRepository => _schoolRepository ??= new SchoolRepository(_context);

    /// <inheritdoc />
    public ISchoolClassRepository SchoolClassRepository => _schoolClassRepository ??= new SchoolClassRepository(_context);

    /// <inheritdoc />
    public ISectionRepository SectionRepository => _sectionRepository ??= new SectionRepository(_context);

    /// <inheritdoc />
    public ISmsLogRepository SmsLogRepository => _smsLogRepository ??= new SmsLogRepository(_context);

    /// <inheritdoc />
    public ISmsTemplateRepository SmsTemplateRepository => _smsTemplateRepository ??= new SmsTemplateRepository(_context);

    /// <inheritdoc />
    public IStudentRepository StudentRepository => _studentRepository ??= new StudentRepository(_context);

    /// <inheritdoc />
    public IStudentAttendanceRepository StudentAttendanceRepository => _studentAttendanceRepository ??= new StudentAttendanceRepository(_context);

    /// <inheritdoc />
    public IStudentGuardianRepository StudentGuardianRepository => _studentGuardianRepository ??= new StudentGuardianRepository(_context);

    /// <inheritdoc />
    public ISubjectRepository SubjectRepository => _subjectRepository ??= new SubjectRepository(_context);

    /// <inheritdoc />
    public ISubjectTeacherRepository SubjectTeacherRepository => _subjectTeacherRepository ??= new SubjectTeacherRepository(_context);

    /// <inheritdoc />
    public ITeacherRepository TeacherRepository => _teacherRepository ??= new TeacherRepository(_context);

    /// <inheritdoc />
    public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);

    /// <inheritdoc />
    public IUserRoleRepository UserRoleRepository => _userRoleRepository ??= new UserRoleRepository(_context);

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task BeginTransactionAsync(
     CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            return;
        }

        _transaction =
            await _context.Database
            .BeginTransactionAsync(cancellationToken);
    }
    public async Task CommitTransactionAsync(
    CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            return;
        }


        try
        {
            await _transaction.CommitAsync(cancellationToken);
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
    public async Task RollbackTransactionAsync(
    CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            return;
        }


        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _context.Dispose();
        }

        _disposed = true;
    }

    public void Dispose()
    {
        _transaction?.Dispose();

        _context.Dispose();
    }
}
