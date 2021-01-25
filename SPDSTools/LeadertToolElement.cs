using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;

namespace SPDSTools
{
public class LeaderToolElement : ToolElement
{


// CadInputQueue.SendCommand "TEXTEDITOR PLAYCOMMAND INSERT_FIELD <Dictionary><Access><String>Area</String></Access><Class><String>MstnClosedBoundary</String></Class><DisplayValue><String>Area</String></DisplayValue><Enabler><String>MstnProp</String></Enabler><InstanceID><String>       0</String></InstanceID><ProviderID><Int>60635</Int></ProviderID><Schema><String>BaseElementSchema</String></Schema></Dictionary><EvaluatorID>ElemProp</EvaluatorID><DependentElement>15EA590100</DependentElement><FormatterName>Area</FormatterName><Formatter><AreaClass xmlns=""MstnPropertyFormatter.01.00""><ShowLabel>1</ShowLabel><Units>-1</Units><PX>AREA = </PX><SX> </SX><UnitDecorator>1</UnitDecorator></AreaClass></Formatter>"


    public int typePointer;

    public string[] text;

        public override int ConstractionPointsCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override ECClassTypeEnum ToolECClassType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        //public List<ElementId> textId;
        //public List<string> just;
        //public List<DSegment3d> lines;

        ///// <summary> Смещение_теста_от_начала_полки </summary>
        //public double deltaX;
        ///// <summary> Минимальная_длина_полки </summary>
        //public double minLenPolka;
        //    //public double deltaX2;  //Вынос_полки_от_конца_текста (параметр не реализован)
        ///// <summary> Вертикальный_отступ_текста_сверху_от_полки </summary>    
        //public double deltaY;

        ////public double deltaY2;   //Вертикальный_отступ_текста_снизу_от_полки (параметр не реализован)

        //// TODO: реализовать функцию изменения размера окружности на выноске
        //public double circleSize;
        //// TODO: реализовать функцию заполнения окружности при circleFilling = true
        //public bool circleFilling;

        //public DPoint3d m_origin;
        //public Angle angle;
        //public long id;
        //public short typeLeader;
        //public int typeDir;
        //public bool flagRestart;
        //public int typeConn;
        //public bool multiline;
        //public bool flagEdit;
        //public long idLine;
        //public int m_step;
        //public Element templ;
        //public short numLineEdit;
        //public long idElement;
        //public long idModel;
        //public string spec;

        public LeaderToolElement(ToolData data): base(data)
        {
            //    // sergbelom: добавил параметры для текста на полке и считывание через переменные окружения AECOSim
            //    CfgVariable.ReadLeaderDataFromProjectWise();

            //    deltaX = CfgVariable.GET_AEP_SPDS_TEXT_GAP_FROM_THE_INITIAL_LANDING;
            //    minLenPolka = CfgVariable.GET_AEP_SPDS_MINIMUM_LANDING_LENGTH;
            //    //deltaX2 = CfgVariable.GET_AEP_SPDS_LEADER_LANDING_FROM_END_TEXT;
            //    deltaY = CfgVariable.GET_AEP_SPDS_TEXT_GAP_TOP_FROM_LANDING;
            //    //deltaY2 = CfgVariable.GET_AEP_SPDS_TEXT_GAP_DOWN_FROM_LANDING;            
            //    circleSize = CfgVariable.GET_AEP_SPDS_CIRCLE_SIZE_ON_LEADER;
            //    circleFilling = CfgVariable.GET_AEP_SPDS_CIRCLE_FILLING_ON_LEADER;

            //    m_textStyle = null;           
            //    pts = new List<DPoint3d>();
            //    typeConn = 0;
            //    flagRestart = true;
            //    text = new List<string>();
            //    just = new List<string>();
            //    multiline = false;
            //    flagEdit = false;
            //    lines = new List<DSegment3d>();
            //    m_step = 0;
            //    templ = null;
            //    numLineEdit = -1;
            //    idElement = 0;
            //    idModel = 0;
            //    spec = "";
            //    textId = new List<ElementId>();
            //}
        }

        protected override CellHeaderElement OnLoadByPoints(List<DPoint3d> points)
        {
            throw new NotImplementedException();
        }

        //protected override void OnLoadFromCell(CellHeaderElement cell, out List<DPoint3d> points)
        //{
        //    throw new NotImplementedException();
        //}

        protected override void OnAfterAddToModel(CellHeaderElement cell)
        {
            throw new NotImplementedException();
        }


        protected override void OnWriteProperties(ECPropertyWriter writer)
        {
            throw new NotImplementedException();
        }

    }
}
