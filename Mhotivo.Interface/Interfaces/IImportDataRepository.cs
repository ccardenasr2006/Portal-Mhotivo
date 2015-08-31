﻿using System.Data;
using System.Web;
using Mhotivo.Data.Entities;

namespace Mhotivo.Interface.Interfaces
{
    public interface IImportDataRepository
    {
        DataSet GetDataSetFromExcelFile(HttpPostedFileBase getFile);
        void Import(DataSet myDataSet, AcademicYear academicYear, IParentRepository parentRepository, IStudentRepository studentRepository, IEnrollRepository enrollRepository, IAcademicYearRepository academicYearRepository, IUserRepository userRepository, IRoleRepository roleRepository);
    }
}