using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Threading;

using CoreLib;

namespace ConsoleApp
{
    class Program
    {
        static void Main()
        {
            // Enabling Resource Monitoring
            AppDomain.MonitoringIsEnabled = true;
            // Disabling Process Memory Counter
            //Application.EnabledPerformanceCounter = false;

            // ����ע�㡿��
            // �о�AppDomain���ڴ�֮������Լ����ͨ��AppDomain����������DLL�������ͷ��ڴ���Դ��ά��Ӧ�ý�����Դƽ��

            // ���ر�˵������
            // ����DLL���н������AppDomain�ڲ����ж����Ʊ���(���л�)�󷵻أ������з��ؽ�������ֽ��������(bytes[])
            // �ⲿ���ѷ��ؽ��ǰ����Ҫ���з����л�����ת����Ϊ�����.NET���� (�ο���Serialize �� Deserialize ����)

            //Dictionary<string, object> outputs = null;
            //var inputs = new Dictionary<string, object>();

            //inputs.Add("message", "Hello Rule");
            //outputs = Application.ExecuteRule("EchoMessageRule", inputs);

            //inputs.Clear(); inputs.Add("count", 1000);
            //outputs = Application.ExecuteRule("GetUserListRule", inputs);

            //var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Rules", "textfile.csv");
            //inputs.Clear(); inputs.Add("path", path);
            //outputs = Application.ExecuteRule("ReadTextFileRule", inputs);

            // ���й۲�AppDomainʵ��������ʱ�䡢�ڴ��ϵ��������
            // ������ @instanceNum Ϊ������AppDomain����
            // �������������@instanceNum=100����˳�����δ���100��AppDomain������¼��������ռ�ڴ����Լ�ÿ�δ���AppDomain�ĺ�ʱ
            //Application.AppDomainCreationMemoryCase(instanceNum: 100);

            // ���й۲�AppDomain��ִ��DLL��ʱ�䡢�ڴ��ϵ��������
            // ������ @ruleType Ϊѡ�����е�DLL���� , @inputs ΪDLL������������� , @taskNum Ϊ���д���
            // ������ѡ��һ��DLL����������������������������@taskNum=100,��˳�δ����µ�AppDomain������ָ��DLL����100�Σ�ÿ��
            // ���н������ͷ�AppDomain������¼��������ռ�ڴ����Լ�ÿ�δ���AppDomain����DLL����ֱ���ͷŽ�����ĺ�ʱ
            //var inputs = new Dictionary<string, object>() { { "count", 1000 } };
            // ������ ����GetUserListRule�����������ڲ��������1000���û����󣬲����������JSON���л��󣬷���JSON�ַ���(�ֽ���)
            //Application.AppDomainSingleExecutionMemoryCase(
            //    ruleType: RuleType.GetUserListRule, inputs: inputs, taskNum: 100);

            // ���й۲칲��AppDomain��ִ��DLL��ʱ�䡢�ڴ��ϵ��������
            // ������ @instanceNum Ϊ�����AppDomain���� ,@ruleType Ϊѡ�����е�DLL���� ,@inputs ΪDLL������������� , @taskNum Ϊ���д���
            // ������Ԥ�ȴ���һ������(����@instanceNum=3)��AppDomainʵ����Ȼ��ѡ��һ��DLL����������������������������@taskNum=100,�������
            // ��Ԥ�ȴ���AppDomain�б���ѡ��һ��AppDomain����ָ��DLL�������������ѡ��->�����С�ѭ��100���������н������ͷ�����AppDomain
            // ����¼��������ռ�ڴ����Լ�ÿ�δ���AppDomain����DLL����ֱ���ͷŽ�����ĺ�ʱ
            //var inputs = new Dictionary<string, object>() { { "count", 1000 } };
            // ������ ����GetUserListRule�����������ڲ��������1000���û����󣬲����������JSON���л��󣬷���JSON�ַ���(�ֽ���)
            //Application.AppDomainSharedExecutionMemoryCase(
            //    instanceNum: 3, ruleType: RuleType.GetUserListRule, inputs: inputs, taskNum: 100);

            Console.Read();
            Application.PrintMemoryUsage();
            Console.ReadLine();
        }
    }







}
