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

            // �ر�˵����
            // ����DLL���н������AppDomain�ڲ����ж����Ʊ���(���л�)�󷵻أ������з��ؽ�������ֽ��������(bytes[])
            // �ⲿ���ѷ��ؽ��ǰ����Ҫ���з����л�����ת����Ϊ�����.NET���� (�ο���Serialize �� Deserialize ����)

            // ���й۲�AppDomainʵ��������ʱ�䡢�ڴ��ϵ��������
            // ������ @instanceNum Ϊ������AppDomain����
            // �������������@sandboxNum=100����˳�����δ���100��AppDomain������¼��������ռ�ڴ����Լ�ÿ�δ���AppDomain�ĺ�ʱ
            //Application.AppDomainCreationMemoryCase(instanceNum: 100);

            // ���й۲�AppDomain��ִ��DLL��ʱ�䡢�ڴ��ϵ��������
            // ������ @ruleType Ϊѡ�����е�DLL���� , @inputs ΪDLL������������� , @taskNum Ϊ���д���
            // ������ѡ��һ��DLL����������������������������@taskNum=100,��˳�δ����µ�AppDomain������ָ��DLL����100�Σ�ÿ��
            // ���н������ͷ�AppDomain������¼��������ռ�ڴ����Լ�ÿ�δ���AppDomain����DLL����ֱ���ͷŽ�����ĺ�ʱ
            //
            // ������ ����GetUserListRule�����������ڲ��������1000���û����󣬲����������JSON���л��󣬷���JSON�ַ���(�ֽ���)
            var inputs = new Dictionary<string, object>() { { "count", 1000 } };
            Application.AppDomainSingleExecutionMemoryCase(
                ruleType: RuleType.GetUserListRule, inputs: inputs, taskNum: 100);

            //var inputs = new Dictionary<string, object>() { { "count", 1000 } };
            //Application.AppDomainSharedExecutionMemoryCase(
            //    instanceNum: 3, ruleType: RuleType.GetUserListRule, inputs: inputs, taskNum: 100);

            //Dictionary<string, object> outputs = null;
            //var inputs = new Dictionary<string, object>();

            //inputs.Add("message", "Hello Rule Activity");
            //outputs = Application.ExecuteRule("MyRuleActivity1", inputs);

            //inputs.Clear(); inputs.Add("count", 1000);
            //outputs = Application.ExecuteRule("GetUserListRule", inputs);

            //var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Rules", "textfile.csv");
            //inputs.Clear(); inputs.Add("path", path);
            //outputs = Application.ExecuteRule("ReadTextFileRule", inputs);

            Application.PrintMemoryUsage();
            Console.ReadLine();
        }
    }







}
