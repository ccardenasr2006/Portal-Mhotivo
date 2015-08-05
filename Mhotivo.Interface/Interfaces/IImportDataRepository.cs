﻿using System.Data;
using Mhotivo.Data.Entities;

namespace Mhotivo.Interface.Interfaces
{
    public interface IImportDataRepository
    {
        DataSet GetDataSetFromExcelFile(string path);
        void Import(DataSet myDataSet, AcademicYear academicYear, IParentRepository parentRepository, IStudentRepository studentRepository, IEnrollRepository enrollRepository, IAcademicYearRepository academicYearRepository, IUserRepository userRepository, IRoleRepository roleRepository);
    }
}
